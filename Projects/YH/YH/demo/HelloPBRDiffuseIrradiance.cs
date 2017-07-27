
using System;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using System.Drawing;
using System.Collections.Generic;
using StbSharp;
using System.IO;

namespace YH
{
	public class HelloPBRDiffuseIrradiance : Application
	{
		public HelloPBRDiffuseIrradiance() : base("HelloPBRDiffuseIrradiance")
		{

		}

		public override void Start(Window wnd)
		{
			base.Start(wnd);

			// configure global opengl state
			// -----------------------------
			//glEnable(GL_DEPTH_TEST);
			//glDepthFunc(GL_LEQUAL); // set depth function to less than AND equal for skybox depth trick.
			GL.Viewport(0, 0, wnd.Width, wnd.Height);
			GL.ClearColor(Color.Black);
			GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Lequal);

			mCamera = new Camera(new Vector3(0.0f, 0.0f, 20.0f), new Vector3(0.0f, 1.0f, 0.0f), -90.0f, Camera.PITCH);
			mCameraController = new CameraController(mAppName, mCamera);

			// build and compile shaders
			// -------------------------
			//Shader pbrShader("2.1.2.pbr.vs", "2.1.2.pbr.fs");
			//Shader equirectangularToCubemapShader("2.1.2.cubemap.vs", "2.1.2.equirectangular_to_cubemap.fs");
			//Shader irradianceShader("2.1.2.cubemap.vs", "2.1.2.irradiance_convolution.fs");
			//Shader backgroundShader("2.1.2.background.vs", "2.1.2.background.fs");
			pbrShader = new GLProgram(@"Resources/2.1.2.pbr.vs", @"Resources/2.1.2.pbr.fs");
			equirectangularToCubemapShader = new GLProgram(@"Resources/2.1.2.cubemap.vs", @"Resources/2.1.2.equirectangular_to_cubemap.fs");
			irradianceShader = new GLProgram(@"Resources/2.1.2.cubemap.vs", @"Resources/2.1.2.irradiance_convolution.fs");
			backgroundShader = new GLProgram(@"Resources/2.1.2.background.vs", @"Resources/2.1.2.background.fs");

            //
            //pbrShader.use();
            //pbrShader.setInt("irradianceMap", 0);
            //pbrShader.setVec3("albedo", 0.5f, 0.0f, 0.0f);
            //pbrShader.setFloat("ao", 1.0f);
            pbrShader.Use();
            GL.Uniform1(pbrShader.GetUniformLocation("irradianceMap"), 0);
            GL.Uniform3(pbrShader.GetUniformLocation("albedo"), 0.5f, 0.0f, 0.0f);
            GL.Uniform1(pbrShader.GetUniformLocation("ao"), 1.0f);

			//backgroundShader.use();
			//backgroundShader.setInt("environmentMap", 0);
			backgroundShader.Use();
			GL.Uniform1(backgroundShader.GetUniformLocation("environmentMap"), 0);

            // pbr: setup framebuffer
            // ----------------------
            //unsigned int captureFBO;
            //unsigned int captureRBO;
            //glGenFramebuffers(1, &captureFBO);
            //glGenRenderbuffers(1, &captureRBO);
            captureFBO = GL.GenFramebuffer();
            captureRBO = GL.GenRenderbuffer();
            //glBindFramebuffer(GL_FRAMEBUFFER, captureFBO);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, captureFBO);
            //glBindRenderbuffer(GL_RENDERBUFFER, captureRBO);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, captureRBO);
			//glRenderbufferStorage(GL_RENDERBUFFER, GL_DEPTH_COMPONENT24, 512, 512);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent24, viewPortSize512, viewPortSize512);
            //glFramebufferRenderbuffer(GL_FRAMEBUFFER, GL_DEPTH_ATTACHMENT, GL_RENDERBUFFER, captureRBO);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, captureRBO);

            // pbr: load the HDR environment map
            // ---------------------------------
            //stbi_set_flip_vertically_on_load(true);
            //int width, height, nrComponents;
            //float* data = stbi_loadf(FileSystem::getPath("resources/textures/hdr/newport_loft.hdr").c_str(), &width, &height, &nrComponents, 0);
            //unsigned int hdrTexture;
            //if (data)
            //{
            //	glGenTextures(1, &hdrTexture);
            //	glBindTexture(GL_TEXTURE_2D, hdrTexture);
            //	glTexImage2D(GL_TEXTURE_2D, 0, GL_RGB16F, width, height, 0, GL_RGB, GL_FLOAT, data); // note how we specify the texture's data value to be float

            //	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_CLAMP_TO_EDGE);
            //	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_CLAMP_TO_EDGE);
            //	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
            //	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);

            //	stbi_image_free(data);
            //}
            //else
            //{
            //	std::cout << "Failed to load HDR image." << std::endl;
            //}
            hdrTexture = LoadTexture(@"Resources/Texture/03-Ueno-Shrine_8k.jpg");

			// pbr: setup cubemap to render to and attach to framebuffer
			// ---------------------------------------------------------
			//unsigned int envCubemap;
			//glGenTextures(1, &envCubemap);
			//glBindTexture(GL_TEXTURE_CUBE_MAP, envCubemap);
            envCubemap = GL.GenTexture();
            GL.BindTexture(TextureTarget.TextureCubeMap, envCubemap);
			for (int i = 0; i < 6; ++i)
			{
				//glTexImage2D(GL_TEXTURE_CUBE_MAP_POSITIVE_X + i, 0, GL_RGB16F, 512, 512, 0, GL_RGB, GL_FLOAT, nullptr);
                GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i,
                              0, PixelInternalFormat.Rgb16f,
                              viewPortSize512, viewPortSize512, 0,
                              PixelFormat.Rgb, PixelType.Float, IntPtr.Zero);
			}

			//glTexParameteri(GL_TEXTURE_CUBE_MAP, GL_TEXTURE_WRAP_S, GL_CLAMP_TO_EDGE);
			//glTexParameteri(GL_TEXTURE_CUBE_MAP, GL_TEXTURE_WRAP_T, GL_CLAMP_TO_EDGE);
			//glTexParameteri(GL_TEXTURE_CUBE_MAP, GL_TEXTURE_WRAP_R, GL_CLAMP_TO_EDGE);
			//glTexParameteri(GL_TEXTURE_CUBE_MAP, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
			//glTexParameteri(GL_TEXTURE_CUBE_MAP, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
			GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
			GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);
			GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)TextureMagFilter.Linear);
			GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

			// pbr: set up projection and view matrices for capturing data onto the 6 cubemap face directions
			// ----------------------------------------------------------------------------------------------
			//glm::mat4 captureProjection = glm::perspective(glm::radians(90.0f), 1.0f, 0.1f, 10.0f);
		    var captureProjection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(90.0f), 1.0f, 0.1f, 10.0f);
            Matrix4[] captureViews =
			{
				//glm::lookAt(glm::vec3(0.0f, 0.0f, 0.0f), glm::vec3(1.0f,  0.0f,  0.0f), glm::vec3(0.0f, -1.0f,  0.0f)),
				//glm::lookAt(glm::vec3(0.0f, 0.0f, 0.0f), glm::vec3(-1.0f,  0.0f,  0.0f), glm::vec3(0.0f, -1.0f,  0.0f)),
				//glm::lookAt(glm::vec3(0.0f, 0.0f, 0.0f), glm::vec3(0.0f,  1.0f,  0.0f), glm::vec3(0.0f,  0.0f,  1.0f)),
				//glm::lookAt(glm::vec3(0.0f, 0.0f, 0.0f), glm::vec3(0.0f, -1.0f,  0.0f), glm::vec3(0.0f,  0.0f, -1.0f)),
				//glm::lookAt(glm::vec3(0.0f, 0.0f, 0.0f), glm::vec3(0.0f,  0.0f,  1.0f), glm::vec3(0.0f, -1.0f,  0.0f)),
				//glm::lookAt(glm::vec3(0.0f, 0.0f, 0.0f), glm::vec3(0.0f,  0.0f, -1.0f), glm::vec3(0.0f, -1.0f,  0.0f))
				Matrix4.LookAt(new Vector3(0.0f, 0.0f, 0.0f), new Vector3(1.0f,  0.0f,  0.0f), new Vector3(0.0f, -1.0f,  0.0f)),
				Matrix4.LookAt(new Vector3(0.0f, 0.0f, 0.0f), new Vector3(-1.0f,  0.0f, 0.0f), new Vector3(0.0f, -1.0f,  0.0f)),
				Matrix4.LookAt(new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f,  1.0f,  0.0f), new Vector3(0.0f,  0.0f,  1.0f)),
				Matrix4.LookAt(new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, -1.0f,  0.0f), new Vector3(0.0f,  0.0f, -1.0f)),
				Matrix4.LookAt(new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f,  0.0f,  1.0f), new Vector3(0.0f, -1.0f,  0.0f)),
				Matrix4.LookAt(new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f,  0.0f, -1.0f), new Vector3(0.0f, -1.0f,  0.0f))
			};

			// pbr: convert HDR equirectangular environment map to cubemap equivalent
			// ----------------------------------------------------------------------
			//equirectangularToCubemapShader.use();
            equirectangularToCubemapShader.Use();
			//equirectangularToCubemapShader.setInt("equirectangularMap", 0);
            GL.Uniform1(equirectangularToCubemapShader.GetUniformLocation("equirectangularMap"), 0);
			//equirectangularToCubemapShader.setMat4("projection", captureProjection);
            GL.UniformMatrix4(equirectangularToCubemapShader.GetUniformLocation("projection"), false, ref captureProjection);
			//glActiveTexture(GL_TEXTURE0);
            GL.ActiveTexture(TextureUnit.Texture0);
			//glBindTexture(GL_TEXTURE_2D, hdrTexture);
            GL.BindTexture(TextureTarget.Texture2D, hdrTexture);
			

            //glViewport(0, 0, 512, 512); // don't forget to configure the viewport to the capture dimensions.
            GL.Viewport(0, 0, viewPortSize512, viewPortSize512);
			//glBindFramebuffer(GL_FRAMEBUFFER, captureFBO);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, captureFBO);
			for (int i = 0; i < 6; ++i)
			{
				//equirectangularToCubemapShader.setMat4("view", captureViews[i]);
				GL.UniformMatrix4(equirectangularToCubemapShader.GetUniformLocation("view"), false, ref captureViews[i]);

				//glFramebufferTexture2D(GL_FRAMEBUFFER, GL_COLOR_ATTACHMENT0, GL_TEXTURE_CUBE_MAP_POSITIVE_X + i, envCubemap, 0);
                GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.TextureCubeMapPositiveX + i, envCubemap, 0);
				//glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                //renderCube();
                RenderCube();
			}
			//glBindFramebuffer(GL_FRAMEBUFFER, 0);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

			// pbr: create an irradiance cubemap, and re-scale capture FBO to irradiance scale.
			// --------------------------------------------------------------------------------
			//unsigned int irradianceMap;
			//glGenTextures(1, &irradianceMap);
            irradianceMap = GL.GenTexture();
			//glBindTexture(GL_TEXTURE_CUBE_MAP, irradianceMap);
            GL.BindTexture(TextureTarget.TextureCubeMap, irradianceMap);
			for (int i = 0; i < 6; ++i)
			{
				//glTexImage2D(GL_TEXTURE_CUBE_MAP_POSITIVE_X + i, 0, GL_RGB16F, 32, 32, 0, GL_RGB, GL_FLOAT, nullptr);
				GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i,
							  0, PixelInternalFormat.Rgb16f,
							  viewPortSize32, viewPortSize32, 0,
							  PixelFormat.Rgb, PixelType.Float, IntPtr.Zero);
			}

			//glTexParameteri(GL_TEXTURE_CUBE_MAP, GL_TEXTURE_WRAP_S, GL_CLAMP_TO_EDGE);
			//glTexParameteri(GL_TEXTURE_CUBE_MAP, GL_TEXTURE_WRAP_T, GL_CLAMP_TO_EDGE);
			//glTexParameteri(GL_TEXTURE_CUBE_MAP, GL_TEXTURE_WRAP_R, GL_CLAMP_TO_EDGE);
			//glTexParameteri(GL_TEXTURE_CUBE_MAP, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
			//glTexParameteri(GL_TEXTURE_CUBE_MAP, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
			GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
			GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
			GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);
			GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)TextureMagFilter.Linear);
			GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);


			//glBindFramebuffer(GL_FRAMEBUFFER, captureFBO);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, captureFBO);
			//glBindRenderbuffer(GL_RENDERBUFFER, captureRBO);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, captureRBO);
            //glRenderbufferStorage(GL_RENDERBUFFER, GL_DEPTH_COMPONENT24, 32, 32);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent24, viewPortSize32, viewPortSize32);

			// pbr: solve diffuse integral by convolution to create an irradiance (cube)map.
			// -----------------------------------------------------------------------------
			//irradianceShader.use();
			//irradianceShader.setInt("environmentMap", 0);
			//irradianceShader.setMat4("projection", captureProjection);
			//glActiveTexture(GL_TEXTURE0);
			//glBindTexture(GL_TEXTURE_CUBE_MAP, envCubemap);
            irradianceShader.Use();
			GL.Uniform1(irradianceShader.GetUniformLocation("environmentMap"), 0);
			GL.UniformMatrix4(irradianceShader.GetUniformLocation("projection"), false, ref captureProjection);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.TextureCubeMap, envCubemap);

            //
            //glViewport(0, 0, 32, 32); // don't forget to configure the viewport to the capture dimensions.
            GL.Viewport(0, 0, viewPortSize32, viewPortSize32);
			//glBindFramebuffer(GL_FRAMEBUFFER, captureFBO);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, captureFBO);
			for (int i = 0; i < 6; ++i)
			{
				//irradianceShader.setMat4("view", captureViews[i]);
				GL.UniformMatrix4(irradianceShader.GetUniformLocation("view"), false, ref captureViews[i]);
				//glFramebufferTexture2D(GL_FRAMEBUFFER, GL_COLOR_ATTACHMENT0, GL_TEXTURE_CUBE_MAP_POSITIVE_X + i, irradianceMap, 0);
                GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.TextureCubeMapPositiveX + i, irradianceMap, 0);
                //glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
				//renderCube();
                RenderCube();
			}
			//glBindFramebuffer(GL_FRAMEBUFFER, 0);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

			// initialize static shader uniforms before rendering
			// --------------------------------------------------
			//glm::mat4 projection = glm::perspective(camera.Zoom, (float)SCR_WIDTH / (float)SCR_HEIGHT, 0.1f, 100.0f);
			projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(mCamera.Zoom),
															(float)wnd.Width / (float)wnd.Height,
															0.1f, 100.0f);
			//pbrShader.use();
            pbrShader.Use();
			//pbrShader.setMat4("projection", projection);
            GL.UniformMatrix4(pbrShader.GetUniformLocation("projection"), false, ref projection);
			//backgroundShader.use();
            backgroundShader.Use();
			//backgroundShader.setMat4("projection", projection);
			GL.UniformMatrix4(backgroundShader.GetUniformLocation("projection"), false, ref projection);

			// then before rendering, configure the viewport to the original framebuffer's screen dimensions
			//int scrWidth, scrHeight;
			//glfwGetFramebufferSize(window, &scrWidth, &scrHeight);
			//glViewport(0, 0, scrWidth, scrHeight);
			GL.Viewport(0, 0, wnd.Width, wnd.Height);
		}

		public override void Update(double dt)
		{
			base.Update(dt);
		}

		public override void Draw(double dt, Window wnd)
		{
			// render
			// ------
			//glClearColor(0.2f, 0.3f, 0.3f, 1.0f);
			//glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);


            // render scene, supplying the convoluted irradiance map to the final shader.
            // ------------------------------------------------------------------------------------------
            //pbrShader.use();
            //glm::mat4 view = camera.GetViewMatrix();
            //pbrShader.setMat4("view", view);
            //pbrShader.setVec3("camPos", camera.Position);
            pbrShader.Use();
            var view = mCamera.GetViewMatrix();
            GL.UniformMatrix4(pbrShader.GetUniformLocation("view"), false, ref view);
			GL.Uniform3(pbrShader.GetUniformLocation("camPos"), mCamera.Position);

			// bind pre-computed IBL data
			//glActiveTexture(GL_TEXTURE0);
            GL.ActiveTexture(TextureUnit.Texture0);
			//glBindTexture(GL_TEXTURE_CUBE_MAP, irradianceMap);
            GL.BindTexture(TextureTarget.TextureCubeMap, irradianceMap);



			const int nrRows = 7;
			const int nrColumns = 7;
            const float spacing = 2.5f;

			// render rows*column number of spheres with material properties defined by textures (they all have the same material properties)
			//glm::mat4 model;
            var model = Matrix4.CreateTranslation(0, 0, 0);
			for (int row = 0; row < nrRows; ++row)
			{
				//pbrShader.setFloat("metallic", (float)row / (float)nrRows);
				GL.Uniform1(pbrShader.GetUniformLocation("metallic"), (float)row / (float)nrRows);

				for (int col = 0; col < nrColumns; ++col)
				{
					// we clamp the roughness to 0.025 - 1.0 as perfectly smooth surfaces (roughness of 0.0) tend to look a bit off
					// on direct lighting.
					//pbrShader.setFloat("roughness", glm::clamp((float)col / (float)nrColumns, 0.05f, 1.0f));
					float v = (float)col / (float)nrColumns;
					v = (v < 0.05f) ? 0.05f : ((v > 1.0f) ? 1.0f : v);
					GL.Uniform1(pbrShader.GetUniformLocation("roughness"), v);
					//model = glm::mat4();
					//model = glm::translate(model, glm::vec3(
					//	(float)(col - (nrColumns / 2)) * spacing,
					//	(float)(row - (nrRows / 2)) * spacing,
					//	-2.0f
					//));
					//pbrShader.setMat4("model", model);
					model = Matrix4.CreateTranslation(
						(float)(col - (nrColumns / 2)) * spacing,
						(float)(row - (nrRows / 2)) * spacing,
						-2.0f);

					GL.UniformMatrix4(pbrShader.GetUniformLocation("model"), false, ref model);

					//renderSphere();
                    RenderSphere();
				}
			}

			// render light source (simply re-render sphere at light positions)
			// this looks a bit off as we use the same shader, but it'll make their positions obvious and 
			// keeps the codeprint small.
            Vector3 newPos = new Vector3();
			//for (unsigned int i = 0; i < sizeof(lightPositions) / sizeof(lightPositions[0]); ++i)
            for (int i = 0; i < lightPositions.Length; ++i)
			{
				//glm::vec3 newPos = lightPositions[i] + glm::vec3(sin(glfwGetTime() * 5.0) * 5.0, 0.0, 0.0);
				//newPos = lightPositions[i];
				//pbrShader.setVec3("lightPositions[" + std::to_string(i) + "]", newPos);
				//pbrShader.setVec3("lightColors[" + std::to_string(i) + "]", lightColors[i]);

				//model = glm::mat4();
				//model = glm::translate(model, newPos);
				//model = glm::scale(model, glm::vec3(0.5f));
				//pbrShader.setMat4("model", model);
				//renderSphere();

				newPos.X = lightPositions[i].X + (float)Math.Sin((float)mTotalRuningTime) * 2.0f * 2.0f;
				newPos.Y = (float)Math.Sin((float)mTotalRuningTime / 2.0f) * 1.0f * 2.0f + lightPositions[i].Y;
				newPos.Z = (float)Math.Sin((float)mTotalRuningTime) * 5.0f;

				GL.Uniform3(pbrShader.GetUniformLocation("lightPositions[" + i + "]"), newPos);
				GL.Uniform3(pbrShader.GetUniformLocation("lightColors[" + i + "]"), lightColors[i]);

				model = Matrix4.CreateTranslation(newPos);
				model = Matrix4.CreateScale(0.5f) * model;
				GL.UniformMatrix4(pbrShader.GetUniformLocation("model"), false, ref model);

				RenderSphere();
			}

            // render skybox (render as last to prevent overdraw)
            //backgroundShader.use();
            //backgroundShader.setMat4("view", view);
            //glActiveTexture(GL_TEXTURE0);
            //glBindTexture(GL_TEXTURE_CUBE_MAP, envCubemap);
            ////glBindTexture(GL_TEXTURE_CUBE_MAP, irradianceMap); // display irradiance map
            //renderCube();
            backgroundShader.Use();
			GL.UniformMatrix4(backgroundShader.GetUniformLocation("view"), false, ref view);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.TextureCubeMap, envCubemap);
            RenderCube();




			/*
			const int nrRows = 7;
			const int nrColumns = 7;
			const float spacing = 2.5f;

			// configure view matrix
			mShader.Use();
			var view = mCamera.GetViewMatrix();
			GL.UniformMatrix4(mShader.GetUniformLocation("view"), false, ref view);

			// setup relevant shader uniforms
			GL.Uniform3(mShader.GetUniformLocation("camPos"), mCamera.Position);

			// set material
			GL.ActiveTexture(TextureUnit.Texture0);
			GL.BindTexture(TextureTarget.Texture2D, mAlbedo.getTextureId());
			GL.ActiveTexture(TextureUnit.Texture1);
			GL.BindTexture(TextureTarget.Texture2D, mNormal.getTextureId());
			GL.ActiveTexture(TextureUnit.Texture2);
			GL.BindTexture(TextureTarget.Texture2D, mMetallic.getTextureId());
			GL.ActiveTexture(TextureUnit.Texture3);
			GL.BindTexture(TextureTarget.Texture2D, mRoughness.getTextureId());
			GL.ActiveTexture(TextureUnit.Texture4);
			GL.BindTexture(TextureTarget.Texture2D, mAO.getTextureId());



			// render rows*column number of spheres with material properties defined by textures (they all have the same material properties)
			var model = Matrix4.CreateTranslation(0, 0, 0);
			for (int row = 0; row < nrRows; ++row)
			{
				for (int col = 0; col < nrColumns; ++col)
				{
					model = Matrix4.CreateTranslation(
						(float)(col - (nrColumns / 2)) * spacing,
						(float)(row - (nrRows / 2)) * spacing,
						0.0f);

					GL.UniformMatrix4(mShader.GetUniformLocation("model"), false, ref model);
					RenderSphere();
				}
			}

			Vector3 newPos = new Vector3();
			for (int i = 0; i < mLightPositions.Length; ++i)
			{
				newPos.X = mLightPositions[i].X + (float)Math.Sin((float)mTotalRuningTime) * 2.0f * 2.0f;
				newPos.Y = (float)Math.Sin((float)mTotalRuningTime / 2.0f) * 1.0f * 2.0f + mLightPositions[i].Y;
				newPos.Z = (float)Math.Sin((float)mTotalRuningTime) * 5.0f;

				GL.Uniform3(mShader.GetUniformLocation("lightPositions[" + i + "]"), newPos);
				GL.Uniform3(mShader.GetUniformLocation("lightColors[" + i + "]"), mLightColors[i]);

				model = Matrix4.CreateTranslation(newPos);
				model = Matrix4.CreateScale(0.5f) * model;
				GL.UniformMatrix4(mShader.GetUniformLocation("model"), false, ref model);

				RenderSphere();
			}
			*/
		}

		private void RenderSphere()
		{
			if (mSphereVAO == 0)
			{
				mSphereVAO = GL.GenVertexArray();

				int vbo = GL.GenBuffer();
				int ebo = GL.GenBuffer();

				List<Vector3> positions = new List<Vector3>();
				List<Vector2> uv = new List<Vector2>();
				List<Vector3> normals = new List<Vector3>();
				List<int> indices = new List<int>();

				const int X_SEGMENTS = 64;
				const int Y_SEGMENTS = 64;
				const float PI = 3.14159265359f;

				for (int y = 0; y <= Y_SEGMENTS; ++y)
				{
					for (int x = 0; x <= X_SEGMENTS; ++x)
					{
						float xSegment = (float)x / (float)X_SEGMENTS;
						float ySegment = (float)y / (float)Y_SEGMENTS;

						float xPos = (float)Math.Cos(xSegment * 2.0f * PI) * (float)Math.Sin(ySegment * PI);
						float yPos = (float)Math.Cos(ySegment * PI);
						float zPos = (float)Math.Sin(xSegment * 2.0f * PI) * (float)Math.Sin(ySegment * PI);

						positions.Add(new Vector3(xPos, yPos, zPos));
						uv.Add(new Vector2(xSegment, ySegment));
						normals.Add(new Vector3(xPos, yPos, zPos));
					}
				}

				bool oddRow = false;
				for (int y = 0; y < Y_SEGMENTS; ++y)
				{
					if (!oddRow)
					{
						for (int x = 0; x <= X_SEGMENTS; ++x)
						{
							indices.Add(y * (X_SEGMENTS + 1) + x);
							indices.Add((y + 1) * (X_SEGMENTS + 1) + x);
						}
					}
					else
					{
						for (int x = X_SEGMENTS; x >= 0; --x)
						{
							indices.Add((y + 1) * (X_SEGMENTS + 1) + x);
							indices.Add(y * (X_SEGMENTS + 1) + x);
						}
					}
					oddRow = !oddRow;
				}

				mIndexCount = indices.Count;

				List<float> data = new List<float>();

				for (int i = 0; i < positions.Count; ++i)
				{
					data.Add(positions[i].X);
					data.Add(positions[i].Y);
					data.Add(positions[i].Z);

					if (uv.Count > 0)
					{
						data.Add(uv[i].X);
						data.Add(uv[i].Y);
					}

					if (normals.Count > 0)
					{
						data.Add(normals[i].X);
						data.Add(normals[i].Y);
						data.Add(normals[i].Z);
					}
				}

				GL.BindVertexArray(mSphereVAO);
				GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
				GL.BufferData(BufferTarget.ArrayBuffer, data.Count * sizeof(float), data.ToArray(), BufferUsageHint.StaticDraw);
				GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
				GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Count * sizeof(int), indices.ToArray(), BufferUsageHint.StaticDraw);

				const int stride = (3 + 2 + 3) * sizeof(float);
				GL.EnableVertexAttribArray(0);
				GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride, IntPtr.Zero);

				GL.EnableVertexAttribArray(1);
				GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, stride, (IntPtr)(3 * sizeof(float)));

				GL.EnableVertexAttribArray(2);
				GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, stride, (IntPtr)(5 * sizeof(float)));

				GL.BindVertexArray(0);
			}

			GL.BindVertexArray(mSphereVAO);
			GL.DrawElements(PrimitiveType.TriangleStrip, mIndexCount, DrawElementsType.UnsignedInt, 0);
			GL.BindVertexArray(0);
		}

		public override void OnKeyUp(OpenTK.Input.KeyboardKeyEventArgs e)
		{
			base.OnKeyUp(e);
		}

		public override void OnKeyDown(OpenTK.Input.KeyboardKeyEventArgs e)
		{
			base.OnKeyDown(e);
		}

		public int LoadTexture(string texPath)
		{
			// pbr: load the HDR environment map
			// ---------------------------------
			//stbi_set_flip_vertically_on_load(true);
			//int width, height, nrComponents;
			//float* data = stbi_loadf(FileSystem::getPath("resources/textures/hdr/newport_loft.hdr").c_str(), &width, &height, &nrComponents, 0);
			//unsigned int hdrTexture;
			//if (data)
			//{
			//  glGenTextures(1, &hdrTexture);
			//  glBindTexture(GL_TEXTURE_2D, hdrTexture);
			//  glTexImage2D(GL_TEXTURE_2D, 0, GL_RGB16F, width, height, 0, GL_RGB, GL_FLOAT, data); // note how we specify the texture's data value to be float

			//  glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_CLAMP_TO_EDGE);
			//  glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_CLAMP_TO_EDGE);
			//  glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
			//  glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);

			//  stbi_image_free(data);
			//}
			//else
			//{
			//  std::cout << "Failed to load HDR image." << std::endl;
			//}
			ImageReader loader = new ImageReader();
			texPath = texPath.Replace(@"\\", @"/");
			texPath = texPath.Replace(@"\", @"/");

			using (System.IO.Stream stream = File.Open(texPath, FileMode.Open))
			{
				Stb.stbi_set_flip_vertically_on_load(1);
				StbSharp.Image image = loader.Read(stream, Stb.STBI_rgb);
				int textureId = GL.GenTexture();
				GL.BindTexture(TextureTarget.Texture2D, textureId);

				GL.TexImage2D(TextureTarget.Texture2D,
								  0,
								  PixelInternalFormat.Rgb16f,
								  image.Width,
								  image.Height,
								  0,
								  OpenTK.Graphics.OpenGL.PixelFormat.Rgb,
								  PixelType.UnsignedByte,
								  image.Data);

				GL.TexParameter(TextureTarget.Texture2D,
								TextureParameterName.TextureWrapS,
								(int)TextureWrapMode.ClampToEdge);

				GL.TexParameter(TextureTarget.Texture2D,
								TextureParameterName.TextureWrapT,
								(int)TextureWrapMode.ClampToEdge);

				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMagFilter.Nearest);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

				GL.BindTexture(TextureTarget.Texture2D, 0);

                return textureId;
			}

            return 0;
		}

		// renderCube() renders a 1x1 3D cube in NDC.
		// -------------------------------------------------
        private int cubeVAO = 0;
		private int cubeVBO = 0;
		void RenderCube()
		{
			// initialize (if necessary)
			if (cubeVAO == 0)
			{
				float[] vertices = {
		            // back face
		            -1.0f, -1.0f, -1.0f,  0.0f,  0.0f, -1.0f, 0.0f, 0.0f, // bottom-left
		             1.0f,  1.0f, -1.0f,  0.0f,  0.0f, -1.0f, 1.0f, 1.0f, // top-right
		             1.0f, -1.0f, -1.0f,  0.0f,  0.0f, -1.0f, 1.0f, 0.0f, // bottom-right         
		             1.0f,  1.0f, -1.0f,  0.0f,  0.0f, -1.0f, 1.0f, 1.0f, // top-right
		            -1.0f, -1.0f, -1.0f,  0.0f,  0.0f, -1.0f, 0.0f, 0.0f, // bottom-left
		            -1.0f,  1.0f, -1.0f,  0.0f,  0.0f, -1.0f, 0.0f, 1.0f, // top-left
		            // front face
		            -1.0f, -1.0f,  1.0f,  0.0f,  0.0f,  1.0f, 0.0f, 0.0f, // bottom-left
		             1.0f, -1.0f,  1.0f,  0.0f,  0.0f,  1.0f, 1.0f, 0.0f, // bottom-right
		             1.0f,  1.0f,  1.0f,  0.0f,  0.0f,  1.0f, 1.0f, 1.0f, // top-right
		             1.0f,  1.0f,  1.0f,  0.0f,  0.0f,  1.0f, 1.0f, 1.0f, // top-right
		            -1.0f,  1.0f,  1.0f,  0.0f,  0.0f,  1.0f, 0.0f, 1.0f, // top-left
		            -1.0f, -1.0f,  1.0f,  0.0f,  0.0f,  1.0f, 0.0f, 0.0f, // bottom-left
		            // left face
		            -1.0f,  1.0f,  1.0f, -1.0f,  0.0f,  0.0f, 1.0f, 0.0f, // top-right
		            -1.0f,  1.0f, -1.0f, -1.0f,  0.0f,  0.0f, 1.0f, 1.0f, // top-left
		            -1.0f, -1.0f, -1.0f, -1.0f,  0.0f,  0.0f, 0.0f, 1.0f, // bottom-left
		            -1.0f, -1.0f, -1.0f, -1.0f,  0.0f,  0.0f, 0.0f, 1.0f, // bottom-left
		            -1.0f, -1.0f,  1.0f, -1.0f,  0.0f,  0.0f, 0.0f, 0.0f, // bottom-right
		            -1.0f,  1.0f,  1.0f, -1.0f,  0.0f,  0.0f, 1.0f, 0.0f, // top-right
		            // right face
		             1.0f,  1.0f,  1.0f,  1.0f,  0.0f,  0.0f, 1.0f, 0.0f, // top-left
		             1.0f, -1.0f, -1.0f,  1.0f,  0.0f,  0.0f, 0.0f, 1.0f, // bottom-right
		             1.0f,  1.0f, -1.0f,  1.0f,  0.0f,  0.0f, 1.0f, 1.0f, // top-right         
		             1.0f, -1.0f, -1.0f,  1.0f,  0.0f,  0.0f, 0.0f, 1.0f, // bottom-right
		             1.0f,  1.0f,  1.0f,  1.0f,  0.0f,  0.0f, 1.0f, 0.0f, // top-left
		             1.0f, -1.0f,  1.0f,  1.0f,  0.0f,  0.0f, 0.0f, 0.0f, // bottom-left     
		            // bottom face
		            -1.0f, -1.0f, -1.0f,  0.0f, -1.0f,  0.0f, 0.0f, 1.0f, // top-right
		             1.0f, -1.0f, -1.0f,  0.0f, -1.0f,  0.0f, 1.0f, 1.0f, // top-left
		             1.0f, -1.0f,  1.0f,  0.0f, -1.0f,  0.0f, 1.0f, 0.0f, // bottom-left
		             1.0f, -1.0f,  1.0f,  0.0f, -1.0f,  0.0f, 1.0f, 0.0f, // bottom-left
		            -1.0f, -1.0f,  1.0f,  0.0f, -1.0f,  0.0f, 0.0f, 0.0f, // bottom-right
		            -1.0f, -1.0f, -1.0f,  0.0f, -1.0f,  0.0f, 0.0f, 1.0f, // top-right
		            // top face
		            -1.0f,  1.0f, -1.0f,  0.0f,  1.0f,  0.0f, 0.0f, 1.0f, // top-left
		             1.0f,  1.0f , 1.0f,  0.0f,  1.0f,  0.0f, 1.0f, 0.0f, // bottom-right
		             1.0f,  1.0f, -1.0f,  0.0f,  1.0f,  0.0f, 1.0f, 1.0f, // top-right     
		             1.0f,  1.0f,  1.0f,  0.0f,  1.0f,  0.0f, 1.0f, 0.0f, // bottom-right
		            -1.0f,  1.0f, -1.0f,  0.0f,  1.0f,  0.0f, 0.0f, 1.0f, // top-left
		            -1.0f,  1.0f,  1.0f,  0.0f,  1.0f,  0.0f, 0.0f, 0.0f  // bottom-left        
		        };

				//glGenVertexArrays(1, &cubeVAO);
                cubeVAO = GL.GenVertexArray();
				//glGenBuffers(1, &cubeVBO);
                cubeVBO = GL.GenBuffer();
				// fill buffer
				//glBindBuffer(GL_ARRAY_BUFFER, cubeVBO);
                GL.BindBuffer(BufferTarget.ArrayBuffer, cubeVBO);
				//glBufferData(GL_ARRAY_BUFFER, sizeof(vertices), vertices, GL_STATIC_DRAW);
                GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * vertices.Length, vertices, BufferUsageHint.StaticDraw);

				// link vertex attributes
				//glBindVertexArray(cubeVAO);
                GL.BindVertexArray(cubeVAO);
				//glEnableVertexAttribArray(0);
                GL.EnableVertexAttribArray(0);
				//glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 8 * sizeof(float), (void*)0);
                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false,  8 * sizeof(float), IntPtr.Zero);
				//glEnableVertexAttribArray(1);
                GL.EnableVertexAttribArray(1);
				//glVertexAttribPointer(1, 3, GL_FLOAT, GL_FALSE, 8 * sizeof(float), (void*)(3 * sizeof(float)));
                GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), (3 * sizeof(float)));
				//glEnableVertexAttribArray(2);
				GL.EnableVertexAttribArray(2);
				//glVertexAttribPointer(2, 2, GL_FLOAT, GL_FALSE, 8 * sizeof(float), (void*)(6 * sizeof(float)));
                GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), (6 * sizeof(float)));
				//glBindBuffer(GL_ARRAY_BUFFER, 0);
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
				//glBindVertexArray(0);
                GL.BindVertexArray(0);
			}
			// render Cube
			//glBindVertexArray(cubeVAO);
			//glDrawArrays(GL_TRIANGLES, 0, 36);
			//glBindVertexArray(0);
			GL.BindVertexArray(cubeVAO);
			GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
			GL.BindVertexArray(0);
		}

		private Camera mCamera = null;

		private Vector3[] lightPositions = {
			new Vector3(-10.0f,  10.0f, 10.0f),
			new Vector3( 10.0f,  10.0f, 10.0f),
			new Vector3(-10.0f, -10.0f, 10.0f),
			new Vector3( 10.0f, -10.0f, 10.0f),
		};

		private Vector3[] lightColors = {
			new Vector3(300.0f, 300.0f, 300.0f),
			new Vector3(300.0f, 300.0f, 300.0f),
			new Vector3(300.0f, 300.0f, 300.0f),
			new Vector3(300.0f, 300.0f, 300.0f)
		};

        //
		private int mSphereVAO = 0;
		private int mIndexCount = 0;

		////
        private GLProgram pbrShader = null;
        private GLProgram equirectangularToCubemapShader = null;
        private GLProgram irradianceShader = null;
        private GLProgram backgroundShader = null;


		private int captureFBO = 0;
		private int captureRBO = 0;
        private int hdrTexture = 0;
        private int envCubemap = 0;

        private Matrix4 projection = new Matrix4();

        private int irradianceMap = 0;

        private int viewPortSize512 = 512;
		private int viewPortSize32 = 32;

	}
}

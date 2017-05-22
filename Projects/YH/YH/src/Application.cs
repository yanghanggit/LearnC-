
namespace YH
{
	public class Application
	{
		public Application(string appName)
		{
			mAppName = appName;
		}

		public virtual void Start()
		{
			mStarted = true;
		}

		public bool isStarted()
		{
			return mStarted;
		}

		public virtual void Update()
		{

		}

		public virtual void Draw(int w, int h)
		{

		}

		private bool mStarted = false;
		public readonly string mAppName = "Application";	
	}
}

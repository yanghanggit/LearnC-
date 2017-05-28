
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

		public virtual void Update(double dt)
		{
			mTotalRuningTime += dt;

			if (mKeyboard != null)
			{
				mKeyboard.Capture();
			}
		}

		public virtual void Draw(double dt, int w, int h)
		{

		}

		private bool mStarted = false;
		public readonly string mAppName = "Application";
		protected double mTotalRuningTime = 0;
		protected Keyboard mKeyboard = null;
	}
}

package md545b7b4a34ee03b715c71d0ff4e712c69;


public class MoveTrailersActivity
	extends android.app.Activity
	implements
		mono.android.IGCUserPeer
{
	static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n" +
			"";
		mono.android.Runtime.register ("THRVI.Core.MoveTrailersActivity, THRVI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", MoveTrailersActivity.class, __md_methods);
	}


	public MoveTrailersActivity () throws java.lang.Throwable
	{
		super ();
		if (getClass () == MoveTrailersActivity.class)
			mono.android.TypeManager.Activate ("THRVI.Core.MoveTrailersActivity, THRVI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onCreate (android.os.Bundle p0)
	{
		n_onCreate (p0);
	}

	private native void n_onCreate (android.os.Bundle p0);

	java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}

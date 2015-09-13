namespace Tekla.Technology.Akit.UserScript 
{
    public class Script
	{
        public static void Run(Tekla.Technology.Akit.IScript akit)
		{
            string XS_Variable = System.Environment.GetEnvironmentVariable("XSBIN");
            string TS_Plugin = @"\applications\tekla\Model\";
            string TS_Application = "AssemblyPrefixCheck.exe";

			if (System.IO.File.Exists(XS_Variable+TS_Plugin+TS_Application))
			{
				System.Diagnostics.Process Process = new System.Diagnostics.Process();
				Process.EnableRaisingEvents=false;
				Process.StartInfo.FileName=XS_Variable+TS_Plugin+TS_Application;
				Process.Start();
				Process.Close();
			}
			else
			{
				System.Windows.Forms.MessageBox.Show(TS_Application+" not found, application stopped!\n\nCheck the files in "+XS_Variable+TS_Plugin,"Tekla Structures", System.Windows.Forms.MessageBoxButtons.OK,System.Windows.Forms.MessageBoxIcon.Error);
			}
        }
    }
}

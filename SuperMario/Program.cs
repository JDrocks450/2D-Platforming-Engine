using System;

namespace SuperMario
{
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //new ControlMapper.Mapper().ShowDialog();
            start:
            using (var game = new Core())
            {
                game.Run();                
            }
            if (Core.RESTART_FLAG)
            {
                Core.RESTART_FLAG = false;
                goto start;
            }
        }
    }
}

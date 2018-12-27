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
            using (var game = new Core())
                game.Run();
        }
    }
}

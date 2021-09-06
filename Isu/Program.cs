using Isu.Services;

namespace Isu
{
    internal class Program
    {
        private static void Main()
        {
            var isu = new IsuService(5);
            isu.AddGroup("M3200");
            var a = isu.FindStudent("fuck");
        }
    }
}

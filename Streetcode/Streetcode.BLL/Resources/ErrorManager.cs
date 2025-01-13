using System.Resources;

namespace Streetcode.BLL.Resources
{
    public static class ErrorManager
    {
        private static readonly ResourceManager ResourceManager = new ResourceManager("Streetcode.BLL.Resources.ErrorResources" ,typeof(ErrorManager).Assembly);
        public static string GetCustomErrorText(string key, params object[] arguments)
        {
            var message = ResourceManager.GetString(key);

            return (message != null)
                ? string.Format(message, arguments) 
                : $"No error for [{key}] key";
        }
    }
}

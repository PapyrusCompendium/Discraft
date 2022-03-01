using System.Runtime.CompilerServices;

using Microsoft.Extensions.Configuration;

namespace Discraft.Services.Interfaces {
    public interface ILogger {
        IConfiguration Configuration { get; }
        LogLevel Level { get; set; }

        void Debug(string debug, [CallerFilePath] string classFile = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string callerName = "");
        void Error(string error, [CallerFilePath] string classFile = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string callerName = "");
        void Info(string info, [CallerFilePath] string classFile = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string callerName = "");
        void Warning(string warning, [CallerFilePath] string classFile = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string callerName = "");
    }
}
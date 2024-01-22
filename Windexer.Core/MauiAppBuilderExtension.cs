using TraceTool;
using Windexer.Core.Managers;
using Windexer.Model;

namespace Windexer.Core
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddWindexerManagers(this IServiceCollection sc)
        {
            return sc             
                .AddDbContext<WindexerContext>()                
                .AddSingleton<DbManager>()
                .AddSingleton<RootFoldersManager>()
                .AddSingleton<IndexEntriesManager>()
                .AddSingleton<IndexationManager>();            
        }
    }
}

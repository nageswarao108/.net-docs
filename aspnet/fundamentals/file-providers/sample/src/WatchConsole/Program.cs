﻿using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace WatchConsole
{
    public class Program
    {
        private static PhysicalFileProvider _fileProvider = 
            new PhysicalFileProvider(Directory.GetCurrentDirectory());
        public static void Main(string[] args)
        {
            Console.WriteLine("Monitoring foo.txt for changes (ctrl-c to quit)...");
            while (true)
            {
                MainAsync().GetAwaiter().GetResult();
            }
        }

        private static async Task MainAsync()
        {
            IChangeToken token = _fileProvider.Watch("foo.txt");
            var tcs = new TaskCompletionSource<object>();
            token.RegisterChangeCallback(state => 
                ((TaskCompletionSource<object>)state).TrySetResult(null), tcs);
            await tcs.Task.ConfigureAwait(false);
            Console.WriteLine("foo.txt changed");
        }
    }
}

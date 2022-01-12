﻿using Albert.Interface;
using Albert.Model;
using Albert.Utilities;
using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ShellProgressBar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Albert.Extensions
{
    [Command("tool",Description= "Some good features that I implemented myself.")]
    public class ToolExtension : ICommand
    {
        private readonly IOptionsSnapshot<ProduceToolEntity> options; //依赖注入可选项
        private readonly ILogger<ToolExtension> loggers; //依赖注入日志服务

        public ToolExtension(IOptionsSnapshot<ProduceToolEntity> options, ILogger<ToolExtension> loggers)
        {
            this.options = options;
            this.loggers = loggers;
        }

        [CommandParameter(0, Description = "")]
        public ToolSupportFunc SupportF { get; set; }

        [CommandOption("Source", 's',Description = "SourcePath")]
        public string SourcePath { get; set; }

        [CommandOption("deleteVersion", 't', Description = "DestinationPath")]
        public string DestinationPath { get; set; }

        public async ValueTask ExecuteAsync(IConsole console)
        {
            console.WithColors(ConsoleColor.White, ConsoleColor.Black);     
            switch (SupportF)
            {
                case ToolSupportFunc.cp:

                    if (string.IsNullOrEmpty(SourcePath) || string.IsNullOrEmpty(DestinationPath))
                    {
                        throw new InvalidOperationException("Please set remote url in configs/producetool.json or environment.");
                    }

                    var copyFilePaths = new List<string>();
                    CopyDirectory(SourcePath, DestinationPath, copyFilePaths);
                    var optionsProgressBar = new ProgressBarOptions
                    {
                        ForegroundColor = ConsoleColor.Yellow,
                        ForegroundColorDone = ConsoleColor.DarkGreen,
                        BackgroundColor = ConsoleColor.DarkGray,
                        BackgroundCharacter = '\u2593'
                    };
                    using (var pbar = new ProgressBar(copyFilePaths.Count, $"Copy", optionsProgressBar))
                    {
                        Parallel.ForEach(copyFilePaths, filePath =>
                        {
                            File.Copy(filePath.Split(",")[0], filePath.Split(",")[1], true);
                            pbar.Tick();
                        });
                    }
                    break;
                case ToolSupportFunc.cptxt:
                    var listCopyPaths = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory+"Configs\\ListCopyPaths.txt");
                    listCopyPaths.ToList().ForEach(path=>{
                        if (string.IsNullOrEmpty(path))
                        {
                            return;
                        }
                    var copyFilePaths = new List<string>();
                    CopyDirectory(path.Split(",")[0], path.Split(",")[1], copyFilePaths);
                    var optionsProgressBar = new ProgressBarOptions
                    {
                        ForegroundColor = ConsoleColor.Yellow,
                        ForegroundColorDone = ConsoleColor.DarkGreen,
                        BackgroundColor = ConsoleColor.DarkGray,
                        BackgroundCharacter = '\u2593'
                    };
                    using (var pbar = new ProgressBar(copyFilePaths.Count, $"Copy", optionsProgressBar))
                    {
                        Parallel.ForEach(copyFilePaths, filePath =>
                        {
                            File.Copy(filePath.Split(",")[0], filePath.Split(",")[1], true);
                            pbar.Tick();
                        });
                    }
                    });
                    break;
                default:
                    goto case ToolSupportFunc.cp;
            }
        }

        private void CopyDirectory(string sourcePath, string destPath,List<string> copyFilePaths)
        {
            string floderName = Path.GetFileName(sourcePath);
            string[] files = Directory.GetFileSystemEntries(sourcePath);
            DirectoryInfo di = new DirectoryInfo(Path.Combine(destPath, floderName));

            if (!Directory.Exists(Path.Combine(destPath, floderName)))
            {
                di = Directory.CreateDirectory(Path.Combine(destPath, floderName));
            }

            foreach (string file in files)
            {
                if (Directory.Exists(file))
                {
                    CopyDirectory(file, di.FullName,copyFilePaths);
                }
                else
                {
                    copyFilePaths.Add(file + "," + Path.Combine(di.FullName, Path.GetFileName(file)));
                }
            }
        }
    }

    public enum ToolSupportFunc
    {
        cp,
        cptxt
    }
}

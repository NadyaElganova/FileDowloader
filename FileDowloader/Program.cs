using FileDowloader;
using System.Diagnostics;

var sw = Stopwatch.StartNew();

var progress = new Progress<double>(p =>
{
    Console.WriteLine(Math.Round(p, 2));
});

FileDowload fileDowload = new FileDowload();
await fileDowload.DowloadFile(
    "https://github.com/rodion-m/SystemProgrammingCourse2022/raw/master/files/payments_19mb.zip", 
    progress);
Console.WriteLine($"Done: {sw.ElapsedMilliseconds}");
fileDowload.Dispose();
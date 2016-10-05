// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Local
// ReSharper disable FieldCanBeMadeReadOnly.Local
// ReSharper disable UnusedMember.Global

using System.Threading;

namespace Logary.Specs
{
  using System.Text;
  using System.IO;

  using Configuration;
  using Machine.Specifications;
  using TextWriter = Targets.TextWriter;

  public class When_using_TimeWithAck
  {
    Establish context = () =>
    {
      writer = new StringWriter(new StringBuilder());
      manager = LogaryFactory.New(
        "Logary Specs",
        with =>
          with.InternalLoggingLevel(LogLevel.Fatal)
              .Target<TextWriter.Builder>("sample string writer",
                  t => t.Target.WriteTo(writer, writer))).Result;
    };

    Cleanup cleanup = () =>
    {
      manager.Dispose();
      writer.Dispose();
    };

    Because reason = () =>
    {
      var logger = manager.GetLogger("TimeWithAck");

      // Action
      logger.TimeWithAck(() => { }, CancellationToken.None, CancellationToken.None, "Action")()
          .Result // wait for buffer
          .Wait(); // wait for promise

      // Func<>
      var func1 = logger.TimeWithAck(() => 32, CancellationToken.None, CancellationToken.None, "Func<>")();
      func1.Item1.ShouldEqual(32);
      func1.Item2
          .Result // wait for buffer
          .Wait(); // wait for promise

      // Func<,>
      var func2 = logger.TimeWithAck<int, int>(i => i, CancellationToken.None, CancellationToken.None, "Func<,>")(10);
      func2.Item1.ShouldEqual(10);
      func2.Item2
        .Result // wait for buffer
        .Wait(); // wait for promise

      subject = writer.ToString();
    };

    It output_should_contain_gauge = () => subject.ShouldContain("gauge");
    It output_should_contain_Action = () => subject.ShouldContain("Action");
    It output_should_contain_Func1 = () => subject.ShouldContain("Func<>");
    It output_should_contain_Func2 = () => subject.ShouldContain("Func<,>");

    static LogManager manager;
    static StringWriter writer;
    static string subject;
  }
}
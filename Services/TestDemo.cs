namespace crud_api.Services;

public interface ITestDemo
{
  int Value1 { get; }
  int Value2 { get; }
}

public class TestDemo : ITestDemo
{
  public int Value1 { get; private set; }
  public int Value2 { get; private set; }

  public TestDemo()
  {
    Value1 = Random.Shared.Next(1, 1001);
    Value2 = Random.Shared.Next(1, 1001);
  }
}
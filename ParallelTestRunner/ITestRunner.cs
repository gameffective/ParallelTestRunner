namespace ParallelTestRunner
{
    public interface ITestRunner
    {
        int ResultCode { get; }

        void Parse(FilterMode filterMode, string filterCategory);
        
        void Execute();
        
        void WriteTrx();
        
        void Clean();
    }
}
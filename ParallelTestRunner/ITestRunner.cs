using System.Collections.Generic;

namespace ParallelTestRunner
{
    public interface ITestRunner
    {
        int ResultCode { get; }

        void Parse(FilterMode filterMode, string filterCategory ,List<string> filterCategories);
        
        void Execute();
        
        void WriteTrx();
        
        void Clean();
    }
}
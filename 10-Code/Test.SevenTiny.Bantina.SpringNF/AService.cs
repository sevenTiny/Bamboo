using SevenTiny.Bantina.Spring;
using SevenTiny.Bantina.Spring.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Test.SevenTiny.Bantina.Spring
{
    public class OperateResult
    {
        public int Code { get; set; }
        public string Message { get; set; }
    }

    public interface IAService
    {
        void ServiceTest();
        void Test();
        int NoArgument();
        void ArgumentVoid(int a, string b);
        bool GetBool(bool bo);
        int GetInt(int age);
        string GetString(string str);
        double GetDouble(double dou);
        decimal GetDecimal(decimal dec);
        float GetFloat(float fl);
        DateTime GetDateTime(DateTime time);
        object GetObject(object obj);
        OperateResult GetOperateResult(int code, string message);
        List<OperateResult> GetOperateResults(List<OperateResult> operateResults);
        void ThrowException();
    }

    [Interceptor]
    public class AService : IAService
    {
        [Service]
        private IBService bService;

        [Action2]
        [Action1]
        public void ServiceTest()
        {
            bService.Test();
        }

        //[Action]
        //[Action2]
        //[Action3]
        public void Test()
        {
            //do nothing;
        }

        //[Action]
        //[Action2]
        //[Action3]
        public bool GetBool(bool bo)
        {
            return bo;
        }

        //[Action]
        public DateTime GetDateTime(DateTime time)
        {
            return time;
        }

        //[Action]
        public decimal GetDecimal(decimal dec)
        {
            return dec;
        }

        //[Action]
        public double GetDouble(double dou)
        {
            return dou;
        }

        //[Action]
        public float GetFloat(float fl)
        {
            return fl;
        }

        //[Action]
        public int GetInt(int age)
        {
            return age;
        }

        //[Action]
        public object GetObject(object obj)
        {
            return obj;
        }

        //[Action]
        public OperateResult GetOperateResult(int code, string message)
        {
            return new OperateResult { Code = code, Message = message };
        }

        //[Action]
        public List<OperateResult> GetOperateResults(List<OperateResult> operateResults)
        {
            return operateResults;
        }

        //[Action]
        public string GetString(string str)
        {
            return str;
        }

        //[Action]
        public void ThrowException()
        {
            //throw new ArgumentException("arguments can not be null");
        }

        //[Action]
        public int NoArgument()
        {
            return 1;
        }

        //[Action]
        public void ArgumentVoid(int a, string b)
        {

        }
    }
}

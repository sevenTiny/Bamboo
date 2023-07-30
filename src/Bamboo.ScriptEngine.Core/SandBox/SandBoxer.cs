////Note:.NET Core 3.0 Preview 5 start support

//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Runtime.Remoting;
//using System.Security;
//using System.Security.Permissions;
//using System.Security.Policy;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using Fasterflect;

//namespace Bamboo.ScriptEngine.SandBox
//{
//    public class SandBoxer
//    {
//        private bool _isClosed;
//        private RunContainer _container;
//        private string _sandBoxerName;
//        private AppDomain _sandboxDomain;

//        static SandBoxer()
//        {
//            AppDomain.MonitoringIsEnabled = true;
//        }

//        public SandBoxer()
//        {
//            CreateSandBoxer();
//        }

//        public SandBoxer(string sandBoxerName)
//        {
//            _sandBoxerName = sandBoxerName;
//            CreateSandBoxer();
//        }

//        private void CreateSandBoxer()
//        {
//            Evidence evidence = new Evidence();
//            evidence.AddHostEvidence(new Zone(SecurityZone.Intranet));
//            PermissionSet permSet = new PermissionSet(PermissionState.None);
//            permSet.AddPermission(new ReflectionPermission(PermissionState.Unrestricted));
//            permSet.AddPermission(
//                new SecurityPermission(SecurityPermissionFlag.Execution | SecurityPermissionFlag.SerializationFormatter));
//            permSet.AddPermission(new FileIOPermission(FileIOPermissionAccess.AllAccess, Path.GetTempPath()));

//            AppDomainSetup adSetup = new AppDomainSetup();
//            adSetup.DisallowCodeDownload = true;
//            adSetup.DisallowBindingRedirects = true;
//            adSetup.DisallowPublisherPolicy = true;
//            adSetup.ApplicationBase = Path.GetFullPath(".");
//            _sandboxDomain =
//                AppDomain.CreateDomain(String.Format("SandBox_{0}_{1}", _sandBoxerName, Guid.NewGuid().ToString("N")),
//                    null, adSetup, permSet, null);

//            var containerType = typeof(RunContainer);
//            ObjectHandle handle = Activator.CreateInstanceFrom(
//                _sandboxDomain,
//                containerType.Assembly.ManifestModule.FullyQualifiedName,
//                containerType.FullName);
//            _container = handle.Unwrap() as RunContainer;
//        }

//        public object ExecuteUntrustedCode(Type type, string methodName, int millisecondsTimeout,
//            params object[] parameters)
//        {
//            object obj = millisecondsTimeout > 0
//                ? _container.ExecuteUntrustedCode(type, methodName, millisecondsTimeout, parameters)
//                : _container.ExecuteUntrustedCode(type, methodName, parameters);
//            SandBoxerResourceStatistics(type.Assembly.FullName, methodName);
//            return obj;
//        }

//        public object ExecuteUntrustedCode(Type type, string methodName,
//            params object[] parameters)
//        {
//            object obj = _container.ExecuteUntrustedCode(type, methodName, parameters);
//            SandBoxerResourceStatistics(type.Assembly.FullName, methodName);
//            return obj;
//        }

//        private void SandBoxerResourceStatistics(string assemblyFullName, string methodName)
//        {
//            /*Tag: language_tenantId_appName_scriptHash*/
//            var builder = new StringBuilder();
//            builder.AppendFormat("Machine: {0},", Environment.MachineName);
//            builder.AppendFormat("Tag: {0}_{1},", assemblyFullName, methodName);
//            builder.AppendFormat("CPU Time: {0},", _sandboxDomain.MonitoringTotalProcessorTime);
//            builder.AppendFormat("Bytes Allocated: {0}",
//                _sandboxDomain.MonitoringTotalAllocatedMemorySize.ToByteSizeString());
//            //_logger.ErrorTrace(builder.ToString());
//        }

//        public void UnloadSandBoxer()
//        {
//            AppDomain.Unload(_sandboxDomain);
//            _isClosed = true;
//        }

//        ~SandBoxer()
//        {
//            if (!_isClosed)
//            {
//                AppDomain.Unload(_sandboxDomain);
//            }
//        }

//    }
//}

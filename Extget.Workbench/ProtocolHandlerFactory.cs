using Extget.Worker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Extget.Workbench {
    public class ProtocolHandlerFactory {

        public static IHandler GetHandler(string filepath) {
            Assembly assembly = null;
            try {
                assembly = Assembly.LoadFile(filepath);
            } catch(Exception) {
                // unable to load as assemly, return null
                return null;
            }

            var iHandlerType = typeof(IHandler);

            IEnumerable<Type> exportedTypes = null;

            try {
                exportedTypes = assembly.GetExportedTypes();
            } catch(NotSupportedException) {
                return null;
            }

            if (exportedTypes == null || exportedTypes.Count() == 0) {
                return null;
            }

            Type handlerType = exportedTypes.Where(type => iHandlerType.IsAssignableFrom(typeof(IHandler)) && IsClassWithParameterLessCtor(type)).FirstOrDefault();

            if(handlerType == null) {
                return null;
            }

            IHandler handlerInstance = null;
            try {
                handlerInstance = (Extget.Worker.IHandler)Activator.CreateInstance(handlerType);
            } catch(Exception) {
                return null;
            }
            return handlerInstance;
        }

        private static bool IsClassWithParameterLessCtor(Type type) {
            return type.IsAbstract == false &&
                type.IsGenericTypeDefinition == false &&
                type.IsInterface == false &&
                !type.IsValueType &&
                type.GetConstructor(Type.EmptyTypes) != null;
              
        }
    }
}

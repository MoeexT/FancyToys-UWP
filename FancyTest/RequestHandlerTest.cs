using System;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace FancyTest;

[TestClass]
public class RequestHandlerTest {
    private interface IStruct { }

    private struct RequestStruct: IStruct {
        public byte[] data;

        public override string ToString() {
            return BitConverter.ToString(data);
        }
    }

    private delegate T RequestResolver<T>(T sct) where T: IStruct;

    private Dictionary<Type, object> _resolvers;

    [TestMethod]
    public void Main() {
        Register<RequestStruct>(DelegateInstance);

        RequestStruct rs = new() {
            data = new byte[] { 1, 2, 3, 4, 5, 6 }
        };
        Console.WriteLine(Request(rs));
    }

    private T Request<T>(T sct) where T: IStruct {
        if (_resolvers.TryGetValue(typeof(T), out object o) && o is RequestResolver<T> resolver) {
            return resolver(sct);
        }
        throw new NullReferenceException("Resolver is null");
    }

    private void Register<T>(RequestResolver<T> resolver) where T: IStruct {
        _resolvers ??= new Dictionary<Type, object>();
        Type type = typeof(T);
        // if (_resolvers.TryGetValue(type, out object o) && o is RequestResolver<T>) return false;
        _resolvers.Add(type, resolver);
    }
    

    private static RequestStruct DelegateInstance(RequestStruct rs) {
        for (int i = 0; i < rs.data.Length; i++) {
            rs.data[i]++;
            Console.WriteLine(rs);
        }

        return rs;
    }

}

using NUnit.Framework;

namespace Mirage.Tests.Weaver
{
    public class SyncVarHookTests : TestsBuildFromTestName
    {
        [Test]
        public void FindsPrivateHook()
        {
            IsSuccess();
        }

        [Test]
        public void FindsPublicHook()
        {
            IsSuccess();
        }

        [Test]
        public void FindsStaticHook()
        {
            IsSuccess();
        }

        [Test]
        public void FindsHookWithNetworkIdentity()
        {
            IsSuccess();
        }

        [Test]
        public void FindsHookWithGameObject()
        {
            IsSuccess();
        }

        [Test]
        public void FindsHookWithOtherOverloadsInOrder()
        {
            IsSuccess();
        }

        [Test]
        public void FindsHookWithOtherOverloadsInReverseOrder()
        {
            IsSuccess();
        }

        static string OldNewMethodFormat(string hookName, string ValueType)
        {
            return string.Format("void {0}({1} oldValue, {1} newValue)", hookName, ValueType);
        }

        [Test]
        public void ErrorWhenNoHookFound()
        {
            HasError($"Could not find hook for 'health', hook name 'onChangeHealth'. Method signature should be {OldNewMethodFormat("onChangeHealth", "System.Int32")}",
                "System.Int32 SyncVarHookTests.ErrorWhenNoHookFound.ErrorWhenNoHookFound::health");
        }

        [Test]
        public void ErrorWhenNoHookWithCorrectParametersFound()
        {
            HasError($"Could not find hook for 'health', hook name 'onChangeHealth'. Method signature should be {OldNewMethodFormat("onChangeHealth", "System.Int32")}",
                "System.Int32 SyncVarHookTests.ErrorWhenNoHookWithCorrectParametersFound.ErrorWhenNoHookWithCorrectParametersFound::health");
        }

        [Test]
        public void ErrorForWrongTypeOldParameter()
        {
            HasError($"Wrong type for Parameter in hook for 'health', hook name 'onChangeHealth'. Method signature should be {OldNewMethodFormat("onChangeHealth", "System.Int32")}",
                "System.Int32 SyncVarHookTests.ErrorForWrongTypeOldParameter.ErrorForWrongTypeOldParameter::health");
        }

        [Test]
        public void ErrorForWrongTypeNewParameter()
        {
            HasError($"Wrong type for Parameter in hook for 'health', hook name 'onChangeHealth'. Method signature should be {OldNewMethodFormat("onChangeHealth", "System.Int32")}",
                "System.Int32 SyncVarHookTests.ErrorForWrongTypeNewParameter.ErrorForWrongTypeNewParameter::health");
        }
    }
}

using System;
using Microsoft.Extensions.DependencyInjection;

namespace ForgeAPI.AspNet
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddForgeAPI(this IServiceCollection collection)
        {
            #region Common Services

            collection.AddSingleton<
                Newtonsoft.Json.Serialization.DefaultContractResolver,
                JSONContractResolver>();

            collection.AddSingleton<
                ForgeAPI.Interface.IForgeAPIConfiguration,
                ForgeAPI.Implementation.CForgeAPIConfiguration>();

            collection.AddSingleton<
                ForgeAPI.Interface.IFactory,
                CFactory>();

            collection.AddSingleton<
                ForgeAPI.Interface.REST.IService,
                ForgeAPI.Implementation.REST.CService>();

            collection.AddSingleton<
                ForgeAPI.Interface.Authentication.IService,
                ForgeAPI.Implementation.Authentication.CService>();

            collection.AddSingleton<
                ForgeAPI.Interface.Utility.IService,
                ForgeAPI.Implementation.Utility.CService>();

            #endregion

            #region Data Management API

            #region Buckets API

            collection.AddSingleton<
                ForgeAPI.Interface.DataManagement.Buckets.IService,
                ForgeAPI.Implementation.DataManagement.Buckets.CService>();

            #region Create Bucket

            collection.AddTransient<
                ForgeAPI.Interface.DataManagement.Buckets.CreateBucket.IInputs,
                ForgeAPI.Implementation.DataManagement.Buckets.CreateBucket.CInputs>();

            collection.AddTransient<
                ForgeAPI.Interface.DataManagement.Buckets.CreateBucket.IOutputs,
                ForgeAPI.Implementation.DataManagement.Buckets.CreateBucket.COutputs>();

            collection.AddTransient<
                ForgeAPI.Interface.DataManagement.Buckets.CreateBucket.IPermissions,
                ForgeAPI.Implementation.DataManagement.Buckets.CreateBucket.CPermissions>();

            #endregion

            #region Delete Bucket

            collection.AddTransient<
                ForgeAPI.Interface.DataManagement.Buckets.DeleteBucket.IInputs,
                ForgeAPI.Implementation.DataManagement.Buckets.DeleteBucket.CInputs>();

            collection.AddTransient<
                ForgeAPI.Interface.DataManagement.Buckets.DeleteBucket.IOutputs,
                ForgeAPI.Implementation.DataManagement.Buckets.DeleteBucket.COutputs>();

            #endregion

            #region Get Buckets

            collection.AddTransient<
                ForgeAPI.Interface.DataManagement.Buckets.GetBuckets.IInputs,
                ForgeAPI.Implementation.DataManagement.Buckets.GetBuckets.CInputs>();

            collection.AddTransient<
                ForgeAPI.Interface.DataManagement.Buckets.GetBuckets.IOutputs,
                ForgeAPI.Implementation.DataManagement.Buckets.GetBuckets.COutputs>();

            collection.AddTransient<
                ForgeAPI.Interface.DataManagement.Buckets.GetBuckets.IBucket,
                ForgeAPI.Implementation.DataManagement.Buckets.GetBuckets.CBucket>();

            #endregion

            #endregion

            #region Objects API

            collection.AddSingleton<
                ForgeAPI.Interface.DataManagement.Objects.IService,
                ForgeAPI.Implementation.DataManagement.Objects.CService>();

            #region Download Object URI

            collection.AddTransient<
                ForgeAPI.Interface.DataManagement.Objects.DownloadObjectURI.IInputs,
                ForgeAPI.Implementation.DataManagement.Objects.DownloadObjectURI.CInputs>();

            collection.AddTransient<
                ForgeAPI.Interface.DataManagement.Objects.DownloadObjectURI.IOutputs,
                ForgeAPI.Implementation.DataManagement.Objects.DownloadObjectURI.COutputs>();

            #endregion

            #region Upload Object

            collection.AddTransient<
                ForgeAPI.Interface.DataManagement.Objects.UploadObject.IInputs,
                ForgeAPI.Implementation.DataManagement.Objects.UploadObject.CInputs>();

            collection.AddTransient<
                ForgeAPI.Interface.DataManagement.Objects.UploadObject.IOutputs,
                ForgeAPI.Implementation.DataManagement.Objects.UploadObject.COutputs>();

            #endregion

            #endregion

            #endregion

            #region Model Derivative API

            #region Derivatives API

            collection.AddSingleton<
                ForgeAPI.Interface.ModelDerivative.Derivatives.IService,
                ForgeAPI.Implementation.ModelDerivative.Derivatives.CService>();

            #region Add Job

            collection.AddTransient<
                ForgeAPI.Interface.ModelDerivative.Derivatives.AddJob.IDestinationSettings,
                ForgeAPI.Implementation.ModelDerivative.Derivatives.AddJob.CDestinationSettings>();

            collection.AddTransient<
                ForgeAPI.Interface.ModelDerivative.Derivatives.AddJob.IInputDefinition,
                ForgeAPI.Implementation.ModelDerivative.Derivatives.AddJob.CInputDefinition>();

            collection.AddTransient<
                ForgeAPI.Interface.ModelDerivative.Derivatives.AddJob.IInputs,
                ForgeAPI.Implementation.ModelDerivative.Derivatives.AddJob.CInputs>();

            collection.AddTransient<
                ForgeAPI.Interface.ModelDerivative.Derivatives.AddJob.IOutputDefinition,
                ForgeAPI.Implementation.ModelDerivative.Derivatives.AddJob.COutputDefinition>();

            collection.AddTransient<
                ForgeAPI.Interface.ModelDerivative.Derivatives.AddJob.IOutputFormat_SVF,
                ForgeAPI.Implementation.ModelDerivative.Derivatives.AddJob.COutputFormat_SVF>();

            collection.AddTransient<
                ForgeAPI.Interface.ModelDerivative.Derivatives.AddJob.IOutputs,
                ForgeAPI.Implementation.ModelDerivative.Derivatives.AddJob.COutputs>();

            #endregion

            #region Get Manifest

            collection.AddTransient<
                ForgeAPI.Interface.ModelDerivative.Derivatives.GetManifest.IDerivative,
                ForgeAPI.Implementation.ModelDerivative.Derivatives.GetManifest.CDerivative>();

            collection.AddTransient<
                ForgeAPI.Interface.ModelDerivative.Derivatives.GetManifest.IDerivativeChild,
                ForgeAPI.Implementation.ModelDerivative.Derivatives.GetManifest.CDerivativeChild>();

            collection.AddTransient<
                ForgeAPI.Interface.ModelDerivative.Derivatives.GetManifest.IInputs,
                ForgeAPI.Implementation.ModelDerivative.Derivatives.GetManifest.CInputs>();

            collection.AddTransient<
                ForgeAPI.Interface.ModelDerivative.Derivatives.GetManifest.IMessage,
                ForgeAPI.Implementation.ModelDerivative.Derivatives.GetManifest.CMessage>();

            collection.AddTransient<
                ForgeAPI.Interface.ModelDerivative.Derivatives.GetManifest.IOutputs,
                ForgeAPI.Implementation.ModelDerivative.Derivatives.GetManifest.COutputs>();

            #endregion

            #endregion

            #endregion

            return collection;
        }
    }
}

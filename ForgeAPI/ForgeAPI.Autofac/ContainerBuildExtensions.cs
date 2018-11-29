using System;
using Microsoft.Extensions.Configuration;
using Autofac;

namespace ForgeAPI.Autofac
{
    public static class ContainerBuilderExtensions
    {
        public static ContainerBuilder AddForgeAPI(this ContainerBuilder builder)
        {
            #region Configuration Provider

            builder.Register(
                ctx =>
                {
                    var configurationBuilder = new ConfigurationBuilder()
                        .AddJsonFile("appsettings.json");

                    return configurationBuilder.Build();
                })
                .As<IConfiguration>();

            #endregion

            #region Common Services

            builder
                .RegisterType<JSONContractResolver>()
                .As<Newtonsoft.Json.Serialization.DefaultContractResolver>()
                .SingleInstance();

            builder
                .RegisterType<ForgeAPI.Implementation.CForgeAPIConfiguration>()
                .As<ForgeAPI.Interface.IForgeAPIConfiguration>()
                .SingleInstance();

            builder
                .RegisterType<CFactory>()
                .As<ForgeAPI.Interface.IFactory>()
                .SingleInstance();

            builder
                .RegisterType<ForgeAPI.Implementation.REST.CService>()
                .As<ForgeAPI.Interface.REST.IService>()
                .SingleInstance();

            builder
                .RegisterType<ForgeAPI.Implementation.Authentication.CService>()
                .As<ForgeAPI.Interface.Authentication.IService>()
                .SingleInstance();

            builder
                .RegisterType<ForgeAPI.Implementation.Utility.CService>()
                .As<ForgeAPI.Interface.Utility.IService>()
                .SingleInstance();

            #endregion

            #region Data Management API

            #region Buckets API

            builder
                .RegisterType<ForgeAPI.Implementation.DataManagement.Buckets.CService>()
                .As<ForgeAPI.Interface.DataManagement.Buckets.IService>()
                .SingleInstance();

            #region Create Bucket

            builder
                .RegisterType<ForgeAPI.Implementation.DataManagement.Buckets.CreateBucket.CInputs>()
                .As<ForgeAPI.Interface.DataManagement.Buckets.CreateBucket.IInputs>();

            builder
                .RegisterType<ForgeAPI.Implementation.DataManagement.Buckets.CreateBucket.COutputs>()
                .As<ForgeAPI.Interface.DataManagement.Buckets.CreateBucket.IOutputs>();

            builder
                .RegisterType<ForgeAPI.Implementation.DataManagement.Buckets.CreateBucket.CPermissions>()
                .As<ForgeAPI.Interface.DataManagement.Buckets.CreateBucket.IPermissions>();

            #endregion

            #region Delete Bucket

            builder
                .RegisterType<ForgeAPI.Implementation.DataManagement.Buckets.DeleteBucket.CInputs>()
                .As<ForgeAPI.Interface.DataManagement.Buckets.DeleteBucket.IInputs>();

            builder
                .RegisterType<ForgeAPI.Implementation.DataManagement.Buckets.DeleteBucket.COutputs>()
                .As<ForgeAPI.Interface.DataManagement.Buckets.DeleteBucket.IOutputs>();

            #endregion

            #region Get Buckets

            builder
                .RegisterType<ForgeAPI.Implementation.DataManagement.Buckets.GetBuckets.CInputs>()
                .As<ForgeAPI.Interface.DataManagement.Buckets.GetBuckets.IInputs>();

            builder
                .RegisterType<ForgeAPI.Implementation.DataManagement.Buckets.GetBuckets.COutputs>()
                .As<ForgeAPI.Interface.DataManagement.Buckets.GetBuckets.IOutputs>();

            builder
                .RegisterType<ForgeAPI.Implementation.DataManagement.Buckets.GetBuckets.CBucket>()
                .As<ForgeAPI.Interface.DataManagement.Buckets.GetBuckets.IBucket>();

            #endregion

            #endregion

            #region Objects API

            builder
                .RegisterType<ForgeAPI.Implementation.DataManagement.Objects.CService>()
                .As<ForgeAPI.Interface.DataManagement.Objects.IService>()
                .SingleInstance();

            #region Download Object URI

            builder
                .RegisterType<ForgeAPI.Implementation.DataManagement.Objects.DownloadObjectURI.CInputs>()
                .As<ForgeAPI.Interface.DataManagement.Objects.DownloadObjectURI.IInputs>();

            builder
                .RegisterType<ForgeAPI.Implementation.DataManagement.Objects.DownloadObjectURI.COutputs>()
                .As<ForgeAPI.Interface.DataManagement.Objects.DownloadObjectURI.IOutputs>();

            #endregion

            #region Upload Object

            builder
                .RegisterType<ForgeAPI.Implementation.DataManagement.Objects.UploadObject.CInputs>()
                .As<ForgeAPI.Interface.DataManagement.Objects.UploadObject.IInputs>();

            builder
                .RegisterType<ForgeAPI.Implementation.DataManagement.Objects.UploadObject.COutputs>()
                .As<ForgeAPI.Interface.DataManagement.Objects.UploadObject.IOutputs>();

            #endregion

            #endregion

            #endregion

            #region Model Derivative API

            #region Derivatives API

            builder
                .RegisterType<ForgeAPI.Implementation.ModelDerivative.Derivatives.CService>()
                .As<ForgeAPI.Interface.ModelDerivative.Derivatives.IService>()
                .SingleInstance();

            #region Add Job

            builder
                .RegisterType<ForgeAPI.Implementation.ModelDerivative.Derivatives.AddJob.CDestinationSettings>()
                .As<ForgeAPI.Interface.ModelDerivative.Derivatives.AddJob.IDestinationSettings>();

            builder
                .RegisterType<ForgeAPI.Implementation.ModelDerivative.Derivatives.AddJob.CInputDefinition>()
                .As<ForgeAPI.Interface.ModelDerivative.Derivatives.AddJob.IInputDefinition>();

            builder
                .RegisterType<ForgeAPI.Implementation.ModelDerivative.Derivatives.AddJob.CInputs>()
                .As<ForgeAPI.Interface.ModelDerivative.Derivatives.AddJob.IInputs>();

            builder
                .RegisterType<ForgeAPI.Implementation.ModelDerivative.Derivatives.AddJob.COutputDefinition>()
                .As<ForgeAPI.Interface.ModelDerivative.Derivatives.AddJob.IOutputDefinition>();

            builder
                .RegisterType<ForgeAPI.Implementation.ModelDerivative.Derivatives.AddJob.COutputFormat_SVF>()
                .As<ForgeAPI.Interface.ModelDerivative.Derivatives.AddJob.IOutputFormat_SVF>();

            builder
                .RegisterType<ForgeAPI.Implementation.ModelDerivative.Derivatives.AddJob.COutputs>()
                .As<ForgeAPI.Interface.ModelDerivative.Derivatives.AddJob.IOutputs>();

            #endregion

            #region Get Manifest

            builder
                .RegisterType<ForgeAPI.Implementation.ModelDerivative.Derivatives.GetManifest.CDerivative>()
                .As<ForgeAPI.Interface.ModelDerivative.Derivatives.GetManifest.IDerivative>();

            builder
                .RegisterType<ForgeAPI.Implementation.ModelDerivative.Derivatives.GetManifest.CDerivativeChild>()
                .As<ForgeAPI.Interface.ModelDerivative.Derivatives.GetManifest.IDerivativeChild>();

            builder
                .RegisterType<ForgeAPI.Implementation.ModelDerivative.Derivatives.GetManifest.CInputs>()
                .As<ForgeAPI.Interface.ModelDerivative.Derivatives.GetManifest.IInputs>();

            builder
                .RegisterType<ForgeAPI.Implementation.ModelDerivative.Derivatives.GetManifest.CMessage>()
                .As<ForgeAPI.Interface.ModelDerivative.Derivatives.GetManifest.IMessage>();

            builder
                .RegisterType<ForgeAPI.Implementation.ModelDerivative.Derivatives.GetManifest.COutputs>()
                .As<ForgeAPI.Interface.ModelDerivative.Derivatives.GetManifest.IOutputs>();

            #endregion

            #endregion

            #endregion

            return builder;
        }
    }
}

﻿using ErrDLogiPTClient.Mod;
using ErrDLogiPTClient.Scene;
using ErrDLogiPTClient.Scene.Sound;
using ErrDLogiPTClient.Scene.UI;
using ErrDLogiPTClient.Service;
using GHEngine;
using GHEngine.IO;
using GHEngine.Logging;
using GHEngine.Screen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrDLogiPTClient.OS;

public abstract class AbstractGameOsDefinition : IGameOSDefinition
{
    // Fields.
    public string Name { get; private init; }
    public string Description { get; private init; }
    public DateTime ReleaseDate { get; private init; }
    public string DefinitionKey { get; private init; }


    // Constructors.
    public AbstractGameOsDefinition(string name, string description, DateTime releaseDate, string definitionKey)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        ReleaseDate = releaseDate;
        DefinitionKey = definitionKey ?? throw new ArgumentNullException(nameof(definitionKey));
    }


    // Protected methods.
    protected abstract IGameOSInstance CreateOSInstance(IGenericServices sceneServices);


    // Private methods.
    private IGenericServices CreateOSServices(IGenericServices sceneServices)
    {
        IGenericServices OSServices = new GlobalServices();

        OSServices.Set<IUserInput>(new DefaultLogiOSUserInput(sceneServices.GetRequired<IUserInput>()));
        OSServices.Set<ILogiSoundEngine>(sceneServices.GetRequired<ILogiSoundEngine>());
        OSServices.Set<ISceneAssetProvider>(sceneServices.GetRequired<ISceneAssetProvider>());
        OSServices.Set<ILogger>(sceneServices.Get<ILogger>());
        OSServices.Set<IDisplay>(sceneServices.Get<IDisplay>());
        OSServices.Set<IModifiableProgramTime>(sceneServices.Get<IModifiableProgramTime>());

        return OSServices;
    }


    // Inherited methods.
    public IGameOSInstance CreateInstance(IGenericServices sceneServices)
    {
        return CreateOSInstance(CreateOSServices(sceneServices));
    }
}

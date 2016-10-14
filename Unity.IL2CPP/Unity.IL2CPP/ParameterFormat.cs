using System;

namespace Unity.IL2CPP
{
	public enum ParameterFormat
	{
		WithTypeAndName,
		WithTypeAndNameNoThis,
		WithType,
		WithTypeNoThis,
		WithName,
		WithTypeAndNameThisObject,
		WithTypeThisObject,
		WithNameNoThis,
		WithNameCastThis,
		WithNameUnboxThis
	}
}

﻿namespace Coffee_machine_console.Resources;

public class Water : Resource
{
    public string valueType = "литров";

    public Water() : base()
    {
    }

    public Water(int value) : base(value)
    {
    }
}
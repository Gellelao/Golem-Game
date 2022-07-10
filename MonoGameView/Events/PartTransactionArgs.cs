using System;
using GolemCore.Models.Part;

namespace MonoGameView.Events;

public class PartTransactionArgs : EventArgs
{
    public Part Part{ get; }

    public PartTransactionArgs(Part part)
    {
        Part = part;
    }
}
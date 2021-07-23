﻿using System.Threading.Tasks;

namespace Cringe.Types
{
    public interface ISocial : IConnectable<PlayerSession, PlayerSession>
    {
    }

    public interface IConnectable<in TC, in TD>
    {
        Task<bool> Connect(TC player);
        bool Disconnect(TD player);
    }
}
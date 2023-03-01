﻿using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Qizilim.az.AppCode.InfraStructure
{
    public class IJsonRequest : IRequest<CommandJsonResponse>
    {
    }
    public interface IJsonRequestHandler<T> : IRequestHandler<T, CommandJsonResponse>
        where T : IRequest<CommandJsonResponse>
    {
    }
}

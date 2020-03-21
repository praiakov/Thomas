﻿using System.Collections.Generic;
using System.Linq;
using Thomas.Business.Interfaces;

namespace Thomas.Business.Notifications
{
    public class Notificador : INotificador
    {
        public List<Notificacao> _notificacoes { get; set; }

        public Notificador()
        {
            _notificacoes = new List<Notificacao>();
        }

        public void Handle(Notificacao notificacao)
        {
            _notificacoes.Add(notificacao);
        }

        public List<Notificacao> ObterNotificacoes()
        {
            return _notificacoes;
        }

        public bool TemNotificacao()
        {
            return _notificacoes.Any();
        }
    }
}

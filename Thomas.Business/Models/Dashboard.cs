﻿namespace Thomas.Business.Models
{
    public class Dashboard
    {
        public int[] Aberto { get; set; }
        public int AbertoCount { get; set; }
        public int[] Fechado { get; set; }
        public int FechadoCount { get; set; }
        public int[] Pausado { get; set; }
        public int PausadoCount { get; set; }
        public int[] Cancelado { get; set; }
        public int CanceladoCount { get; set; }
        public string[] Mes { get; set; }

    }
}

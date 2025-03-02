using Entidades;

namespace ApiBase.DTOs
{
    public class PedidoDTO
    {
        public string brokers { get; set; }
        public string estado { get; set; }
        public string banco { get; set; }
        public DateTime? fecha { get; set; }
        public string empleado { get; set; }
        public string monto { get; set; }

        public Pedidos PedidoParaBase()
        {
            var pedido = new Pedidos();
            pedido.FechaCreacion = this.fecha;
            pedido.Agente = Guid.NewGuid();
            pedido.Monto = decimal.Parse(this.monto);
            pedido.Broker = int.Parse( this.brokers);
            pedido.IdExterno = Guid.NewGuid();
            pedido.Estado = int.Parse(estado);
            return pedido;
        }
    }
}

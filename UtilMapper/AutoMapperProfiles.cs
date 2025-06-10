using AutoMapper;
using DBModel.DB;
using Models.RequestRequest;
using Models.RequestResponse;
using Models.ResponseResponse;

namespace UtilMapper
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
        
            CreateMap<Autor, AutorRequest>().ReverseMap();
            CreateMap<Autor, AutorResponse>().ReverseMap();

            CreateMap<Libro, LibroRequest>().ReverseMap();
            CreateMap<Libro, LibroResponse>().ReverseMap();

            CreateMap<Usuario, UsuarioRequest>().ReverseMap();  
            CreateMap<Usuario, UsuarioResponse>().ReverseMap();

            CreateMap<Categoria, CategoriaRequest>().ReverseMap();
            CreateMap<Categoria, CategoriaResponse>().ReverseMap();
    
            CreateMap<Usuario, UsuarioRequest>().ReverseMap();
            CreateMap<Usuario, UsuarioResponse>().ReverseMap();

            CreateMap<Persona, PersonaRequest>().ReverseMap();
            CreateMap<Persona, PersonaResponse>().ReverseMap();

    

            CreateMap<LibroAutor, LibroAutorRequest>().ReverseMap();
            CreateMap<LibroAutor, LibroAutorResponse>().ReverseMap();

            CreateMap<Venta, VentaRequest>().ReverseMap();
            CreateMap<Venta, VentaResponse>().ReverseMap();

            CreateMap<DatosGenerale, DatosGeneraleRequest>().ReverseMap();
            CreateMap<DatosGenerale, DatosGeneraleResponse>().ReverseMap();

            CreateMap<Caja, CajaRequest>().ReverseMap();
            CreateMap<Caja,CajaResponse>().ReverseMap();



            CreateMap<DetalleDocSalida,DetalleDocSalidaRequest>().ReverseMap();
            CreateMap<DetalleDocSalida,DetalleDocSalidaResponse>().ReverseMap();

            CreateMap<DetalleVenta,DetalleVentaRequest>().ReverseMap();
            CreateMap<DetalleVenta,DetalleVentaResponse>().ReverseMap();


            CreateMap< DocSalida, DocSalidaRequest>().ReverseMap();
            CreateMap<DocSalida,  DocSalidaResponse>().ReverseMap();    

            CreateMap<Kardex, KardexRequest>().ReverseMap();
            CreateMap<Kardex , KardexResponse>().ReverseMap();

            CreateMap<Precio, PrecioRequest>().ReverseMap();
            CreateMap<Precio , PrecioResponse>().ReverseMap();

            CreateMap<Proveedor, ProveedorRequest>().ReverseMap();
            CreateMap<Proveedor , ProveedorResponse>().ReverseMap();

            CreateMap<Sucursal, SucursalRequest>().ReverseMap();
            CreateMap<Sucursal , SucursalResponse>().ReverseMap();

            CreateMap<TipoDocSalida,TipoDocSalidaRequest>().ReverseMap();
            CreateMap<TipoDocSalida,TipoDocSalidaResponse>().ReverseMap();

            CreateMap<TipoPapel, TipoPapelRequest>().ReverseMap();
            CreateMap<TipoPapel, TipoPapelResponse>().ReverseMap();

            CreateMap<Subcategoria, SubcategoriaRequest>().ReverseMap();
            CreateMap<Subcategoria, SubcategoriaResponse>().ReverseMap();

            CreateMap<PublicoObjetivo,PublicoObjetivoRequest>().ReverseMap();
            CreateMap<PublicoObjetivo, PublicoObjetivoResponse>().ReverseMap();


            CreateMap<DetalleVentaResponse, DetalleVentaRequest>();
            //CreateMap<CajaResponse, CajaRequest>();

            CreateMap<RetiroDeCajaRequest, RetiroDeCaja>();
            CreateMap<RetiroDeCaja, RetiroDeCajaResponse>();

            CreateMap<PedidoProveedor,PedidoProveedorRequest>().ReverseMap();
            CreateMap<PedidoProveedor,PedidoProveedorResponse>().ReverseMap();

            CreateMap<DetallePedidoProveedor, DetallePedidoProveedorRequest>().ReverseMap();
            CreateMap<DetallePedidoProveedor, DetallePedidoProveedorResponse>().ReverseMap();
        }

    }
}

namespace API.DTOs
{
    public class DocumentoDTO : DatosUsuarioDTO
    {
        //public string Id { get; set; }
        public string path { get; set; }
        public string preview{ get; set; }
        public string name { get; set; }
        public string lastModified { get; set; }
        public string webkitRelativePath { get; set; }
        public string size { get; set; }
        public string type { get; set; }


        //Campos para mandar a la api de documentos
        public string DocumentBase64 { get; set; }
        public string systemName { get; set; }
        public string DocumentType { get; set; }
        public string NameFile { get; set; }
        public string Folio { get; set; }
        public string Origin { get; set; }
        public string FileName { get; set; }
        public int ? Status { get; set; }
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid IdentificationId { get; set; }
        public Guid documentId { get; set; }

    }
}

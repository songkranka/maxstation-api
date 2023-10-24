namespace Vatno.Worker.Dto;

public class BranchDto
{
    public string BrnCode { get; set; }
    public string BrnName { get; set; }
    public string Address { get; set; }
    public string PostCode { get; set; }
    public string Phone { get; set; }
    public string BranchNo { get; set; }
    public string CompCode { get; set; }
}

public class CompanyDto
{
    public string CompCode { get; set; }
    public string CompSname { get; set; }
    public string CompName { get; set; }
    public string Address { get; set; }
    public string Phone { get; set; }
    public string Fax { get; set; }
    public string RegisterId { get; set; }
    public string CompNameEn { get; set; }
    public string AddressEn { get; set; }
}
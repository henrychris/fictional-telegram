using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using api.Objects.AnyLevel;
using api.Objects.CompanyLevel;
using api.Objects.RetailLevel;
using api.ReportClasses.BranchLevel;
using api.ReportClasses.CompanyLevel;
using System.ComponentModel.DataAnnotations;
using api.ReportClasses.CompanyLevel.Management;
using Newtonsoft.Json;
using api.ReportClasses.CompanyLevel.Retainerships;
using api.ReportClasses.Summaries;
using System.Text.Json;

namespace api.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class EpumpReportController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JsonCon _jsonCon;

        public EpumpReportController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _jsonCon = new JsonCon();
        }

        [HttpGet("Branch/BranchCashFlowReport")]

        public async Task<ActionResult> GenerateBranchCashflowReport([FromQuery][Required] string companyId, [FromQuery][Required] string branchId, [FromQuery] string startDate, [FromQuery] string endDate)
        {
            var client = _httpClientFactory.CreateClient("EpumpApi");
            var authHeader = Request.Headers["Authorization"];
            client.DefaultRequestHeaders.Add("Authorization", $"{authHeader}");

            var companyResponse = await client.GetAsync($"/Company/Branches/{companyId}");
            var cashflowResponse = await client.GetAsync($"/Settlement/GetBranchCashFlowReport/{branchId}?date={startDate}&enddate={endDate}");

            if (companyResponse.IsSuccessStatusCode && cashflowResponse.IsSuccessStatusCode)
            {
                var pdf = new BranchCashFlow();

                var branchDetails = _jsonCon.deSerializeAsList<BranchDetails>(await companyResponse.Content.ReadAsStringAsync());
                var cashflowList = _jsonCon.deSerializeAsList<Cashflow>(await cashflowResponse.Content.ReadAsStringAsync());
                var pdfResponse = pdf.GetPdf(branchId, branchDetails, cashflowList);

                return File(pdfResponse, "application/octet-stream", "branch-cashflow-report.pdf");
            }
            else
            {
                // Internal Server Error.
                return StatusCode(500);
            }
        }

        [HttpGet("Branch/BranchProductSummary")]

        public async Task<ActionResult<Summary>> GenerateBranchProductSummary([FromQuery][Required] string companyId, [FromQuery][Required] string branchId, [FromQuery] string startDate, [FromQuery] string endDate)
        {
            var client = _httpClientFactory.CreateClient("EpumpApi");
            var authHeader = Request.Headers["Authorization"];
            client.DefaultRequestHeaders.Add("Authorization", $"{authHeader}");

            var companyBranchResponse = await client.GetAsync($"/Company/Branches/{companyId}");
            var summaryResponse = await client.GetAsync($"/Branch/Sales/{branchId}?date={startDate}&enddate={endDate}");

            if (companyBranchResponse.IsSuccessStatusCode && summaryResponse.IsSuccessStatusCode)
            {
                var pdf = new ProductSummary();

                var branchDetails = _jsonCon.deSerializeAsList<BranchDetails>(await companyBranchResponse.Content.ReadAsStringAsync());
                var salesList = _jsonCon.deSerializeAsList<Sales>(await summaryResponse.Content.ReadAsStringAsync());
                var pdfResponse = pdf.getPdf(branchId, branchDetails, salesList);

                return pdfResponse;
            }
            else
            {
                return StatusCode(500);
            }
        }

        [HttpGet("Branch/BranchSalesTransactionReport")]

        public async Task<ActionResult<Summary>> GenerateBranchSalesTransactionReport([FromQuery][Required] string companyId, [FromQuery][Required] string branchId, [FromQuery] string startDate, [FromQuery] string endDate)
        {
            var client = _httpClientFactory.CreateClient("EpumpApi");
            var authHeader = Request.Headers["Authorization"];
            client.DefaultRequestHeaders.Add("Authorization", $"{authHeader}");

            var companyBranchResponse = await client.GetAsync($"/Company/Branches/{companyId}");
            var salesResponse = await client.GetAsync($"/Branch/Sales/{branchId}?startDate={startDate}&endDate={endDate}");

            if (companyBranchResponse.IsSuccessStatusCode && salesResponse.IsSuccessStatusCode)
            {
                var pdf = new BranchSalesTransactions();

                var branchDetails = _jsonCon.deSerializeAsList<BranchDetails>(await companyBranchResponse.Content.ReadAsStringAsync());
                var branchSalesList = _jsonCon.deSerializeAsList<Sales>(await salesResponse.Content.ReadAsStringAsync());
                var pdfResponse = pdf.getPdf(endDate, branchId, branchDetails, branchSalesList);

                return pdfResponse;
            }
            else
            {
                return StatusCode(500);
            }
        }

        [HttpGet("Branch/BranchTanksFilledReport")]
        public async Task<ActionResult<Summary>> GenerateBranchTanksFilledReport([FromQuery][Required] string companyId, [FromQuery][Required] string branchId, [FromQuery][Required] string date)
        {
            var client = _httpClientFactory.CreateClient("EpumpApi");
            var authHeader = Request.Headers["Authorization"];
            client.DefaultRequestHeaders.Add("Authorization", $"{authHeader}");

            var companyResponse = await client.GetAsync($"/Company/Branches/{companyId}");
            var tanksResponse = await client.GetAsync($"/Branch/TanksFilled/{branchId}?date={date}");

            if (companyResponse.IsSuccessStatusCode && tanksResponse.IsSuccessStatusCode)
            {
                var pdf = new BranchTanksFilled();

                var branchDetails = _jsonCon.deSerializeAsList<BranchDetails>(await companyResponse.Content.ReadAsStringAsync());
                var tanksList = _jsonCon.deSerializeAsList<TanksFilled>(await tanksResponse.Content.ReadAsStringAsync());
                var pdfResponse = pdf.getPdf(branchId, branchDetails, tanksList, date);

                return pdfResponse;
            }
            else
            {
                return StatusCode(500);
            }
        }

        [HttpGet("Branch/BranchVarianceReport")]

        public async Task<ActionResult<Summary>> GenerateBranchVarianceReport([FromQuery][Required] string companyId, [FromQuery][Required] string branchId, [FromQuery] string startDate, [FromQuery] string endDate)
        {
            var client = _httpClientFactory.CreateClient("EpumpApi");
            var authHeader = Request.Headers["Authorization"];
            client.DefaultRequestHeaders.Add("Authorization", $"{authHeader}");

            var companyBranchResponse = await client.GetAsync($"/Company/Branches/{companyId}");
            var varianceResponse = await client.GetAsync($"/Pumps/BranchVariance/{branchId}?date={startDate}&enddate={endDate}");

            if (companyBranchResponse.IsSuccessStatusCode && varianceResponse.IsSuccessStatusCode)
            {
                var pdf = new BranchVariance();

                var branchDetails = _jsonCon.deSerializeAsList<BranchDetails>(await companyBranchResponse.Content.ReadAsStringAsync());
                var branchSalesList = _jsonCon.deSerializeAsList<Variance>(await varianceResponse.Content.ReadAsStringAsync());
                var pdfResponse = pdf.getPdf(branchId, branchDetails, branchSalesList);

                return pdfResponse;
            }
            else
            {
                return StatusCode(500);
            }
        }

        [HttpGet("Branch/TankReport")]

        public async Task<ActionResult> GenerateTankReport([FromQuery][Required] string companyId, [FromQuery][Required] string branchId, [FromQuery] string startDate, [FromQuery] string endDate)
        {
            var client = _httpClientFactory.CreateClient("EpumpApi");
            var authHeader = Request.Headers["Authorization"];
            client.DefaultRequestHeaders.Add("Authorization", $"{authHeader}");

            var companyBranchResponse = await client.GetAsync($"/Company/Branches/{companyId}");
            var reportResponse = await client.GetAsync($"/Branch/TankSales/{branchId}?date={startDate}&enddate={endDate}");

            if (companyBranchResponse.IsSuccessStatusCode && reportResponse.IsSuccessStatusCode)
            {
                var pdf = new TankReport();

                var branchDetails = _jsonCon.deSerializeAsList<BranchDetails>(await companyBranchResponse.Content.ReadAsStringAsync());
                var tankReportList = _jsonCon.deSerializeAsList<TankReportObj>(await reportResponse.Content.ReadAsStringAsync());
                var pdfResponse = pdf.getPdf(branchId, branchDetails, tankReportList, startDate, endDate);

                return File(pdfResponse, "application/octet-stream", "branch-tank-report.pdf");
            }
            else
            {
                return StatusCode(500);
            }
        }

        // Company Level Reports 

        [HttpGet("Company/CompanyBranchSalesReport")]

        public async Task<ActionResult<Summary>> GenerateCompanyBranchSalesReport([FromQuery][Required] string companyId, [FromQuery] string startDate, [FromQuery] string endDate)
        {
            var client = _httpClientFactory.CreateClient("EpumpApi");
            var authHeader = Request.Headers["Authorization"];
            client.DefaultRequestHeaders.Add("Authorization", $"{authHeader}");

            var companyResponse = await client.GetAsync($"/Company/MyCompanies");
            var branchSalesResponse = await client.GetAsync($"/Company/CompanyBranchSales/{companyId}/{startDate}/{endDate}");

            if (companyResponse.IsSuccessStatusCode && branchSalesResponse.IsSuccessStatusCode)
            {
                var pdf = new CompanyBranchSales();

                var companyList = _jsonCon.deSerializeAsList<Company>(await companyResponse.Content.ReadAsStringAsync());
                var branchSalesList = _jsonCon.deSerializeAsList<SalesPerBranch>(await branchSalesResponse.Content.ReadAsStringAsync());
                var pdfResponse = pdf.getPdf(companyList, branchSalesList);

                return pdfResponse;
            }
            else
            {
                return StatusCode(500);
            }
        }

        [HttpGet("Company/CompanyCashflowReport")]

        public async Task<IActionResult> GenerateCompanyCashflowReport([FromQuery][Required] string companyId, [FromQuery] string startDate, [FromQuery] string endDate)
        {
            var client = _httpClientFactory.CreateClient("EpumpApi");
            var authHeader = Request.Headers["Authorization"];
            client.DefaultRequestHeaders.Add("Authorization", $"{authHeader}");

            var companyResponse = await client.GetAsync($"/Company/MyCompanies");
            var cashflowResponse = await client.GetAsync($"/Settlement/GetCompanyCashFlowReport/{companyId}?date={startDate}&enddate={endDate}");

            if (companyResponse.IsSuccessStatusCode && cashflowResponse.IsSuccessStatusCode)
            {
                var pdf = new CompanyCashflow();

                var companyList = _jsonCon.deSerializeAsList<Company>(await companyResponse.Content.ReadAsStringAsync());
                var cashflowList = _jsonCon.deSerializeAsList<Cashflow>(await cashflowResponse.Content.ReadAsStringAsync());
                var pdfResponse = pdf.getPdf(companyList, cashflowList);

                return File(pdfResponse, "application/octet-stream", "company-cashflow-report.pdf");
            }
            else
            {
                return StatusCode(500);
            }
        }

        [HttpGet("Company/CompanySalesSummaryReport")]

        public async Task<ActionResult<Summary>> GenerateCompanySalesSummaryReport([FromQuery][Required] string companyId, [FromQuery] string startDate, [FromQuery] string endDate)
        {
            var pdf = new CompanySalesSummary();

            var client = _httpClientFactory.CreateClient("EpumpApi");
            var authHeader = Request.Headers["Authorization"];
            client.DefaultRequestHeaders.Add("Authorization", $"{authHeader}");

            var companyResponse = await client.GetAsync($"/Company/MyCompanies");
            var saleSummaryResponse = await client.GetAsync($"/Dashboard/CompanySaleSummary/{companyId}?startdate={startDate}&enddate={endDate}");

            if (companyResponse.IsSuccessStatusCode && saleSummaryResponse.IsSuccessStatusCode)
            {
                var companyList = _jsonCon.deSerializeAsList<Company>(await companyResponse.Content.ReadAsStringAsync());
                var saleSummaryList = _jsonCon.deSerializeAsList<saleSummary>(await saleSummaryResponse.Content.ReadAsStringAsync());
                var pdfResponse = pdf.getPDF(companyList, saleSummaryList, startDate, endDate);

                return pdfResponse;
            }
            else
            {
                return StatusCode(500);
            }
        }

        [HttpGet("Company/CompanyTankStockReport")]

        public async Task<ActionResult<Summary>> GenerateCompanyTankStockResult([FromQuery][Required] string companyId)
        {
            var client = _httpClientFactory.CreateClient("EpumpApi");
            var authHeader = Request.Headers["Authorization"];
            client.DefaultRequestHeaders.Add("Authorization", $"{authHeader}");

            var companyResponse = await client.GetAsync($"/Company/MyCompanies");
            var tankStockResponse = await client.GetAsync($"/Dashboard/TankStock/{companyId}");

            if (companyResponse.IsSuccessStatusCode && tankStockResponse.IsSuccessStatusCode)
            {
                var pdf = new CompanyTankStock();

                var companyList = _jsonCon.deSerializeAsList<Company>(await companyResponse.Content.ReadAsStringAsync());
                var tankStockList = _jsonCon.deSerializeAsList<TankStock>(await tankStockResponse.Content.ReadAsStringAsync());
                var pdfResponse = pdf.getPdf(companyList, tankStockList);

                return pdfResponse;
            }
            else
            {
                return StatusCode(500);
            }
        }

        [HttpGet("Company/CompanyTanksFilledReport")]

        public async Task<ActionResult<Summary>> GenerateCompanyTanksFilledReport([FromQuery][Required] string companyId, [FromQuery][Required] string date)
        {
            var pdf = new CompanyTanksFilled();

            var client = _httpClientFactory.CreateClient("EpumpApi");
            var authHeader = Request.Headers["Authorization"];
            client.DefaultRequestHeaders.Add("Authorization", $"{authHeader}");

            var companyResponse = await client.GetAsync($"/Company/MyCompanies");
            var tanksResponse = await client.GetAsync($"/Company/TanksFilled/{companyId}?date={date}");

            if (companyResponse.IsSuccessStatusCode && tanksResponse.IsSuccessStatusCode)
            {
                var companyList = _jsonCon.deSerializeAsList<Company>(await companyResponse.Content.ReadAsStringAsync());
                var tanksList = _jsonCon.deSerializeAsList<TanksFilled>(await tanksResponse.Content.ReadAsStringAsync());
                var pdfResponse = pdf.getPdf(tanksList, companyList, date);

                return pdfResponse;
            }
            else
            {
                return StatusCode(500);
            }
        }

        [HttpGet("Company/CompanyVarianceReport")]

        public async Task<ActionResult<Summary>> GenerateCompanyVarianceReport([FromQuery][Required] string companyId, [FromQuery] string startDate, [FromQuery] string endDate)
        {
            var pdf = new CompanyVariance();

            var client = _httpClientFactory.CreateClient("EpumpApi");
            var authHeader = Request.Headers["Authorization"];
            client.DefaultRequestHeaders.Add("Authorization", $"{authHeader}");

            var companyResponse = await client.GetAsync($"/Company/MyCompanies");
            var varianceResponse = await client.GetAsync($"/Pumps/CompanyVariance/{companyId}?startDate={startDate}&endDate={endDate}");

            if (companyResponse.IsSuccessStatusCode && varianceResponse.IsSuccessStatusCode)
            {
                var companyList = _jsonCon.deSerializeAsList<Company>(await companyResponse.Content.ReadAsStringAsync());
                var varianceList = _jsonCon.deSerializeAsList<Variance>(await varianceResponse.Content.ReadAsStringAsync());
                var pdfResponse = pdf.getPdf(companyList, varianceList);

                return pdfResponse;
            }
            else
            {
                return StatusCode(500);
            }
        }

        [HttpGet("Company/Management/ExpenseCategoriesReport")]
        public async Task<IActionResult> GenerateExpenseCategoriesReport([FromQuery][Required] string companyId)
        {
            var pdf = new ExpenseCategoriesReport();

            var client = _httpClientFactory.CreateClient("EpumpApi");
            var authHeader = Request.Headers["Authorization"];
            client.DefaultRequestHeaders.Add("Authorization", $"{authHeader}");

            var companyResponse = await client.GetAsync($"/Company/MyCompanies");
            var expenseResponse = await client.GetAsync($"/Expenses/Categories/{companyId}");

            if (companyResponse.IsSuccessStatusCode && expenseResponse.IsSuccessStatusCode)
            {
                var companyList = _jsonCon.deSerializeAsList<Company>(await companyResponse.Content.ReadAsStringAsync());
                var expenseList = _jsonCon.deSerializeAsList<ExpenseCategories>(await expenseResponse.Content.ReadAsStringAsync());
                var pdfResponse = pdf.getPdf(companyList, expenseList);

                return File(pdfResponse, "application/octet-stream", "expense-categories-report.pdf");
            }
            else
            {
                return StatusCode(500);
            }
        }

        [HttpGet("Company/Management/ZonesReport")]

        public async Task<IActionResult> GenerateManagementZonesReport([FromQuery][Required] string companyId)
        {
            var pdf = new ZonesReport();

            var client = _httpClientFactory.CreateClient("EpumpApi");
            var authHeader = Request.Headers["Authorization"];
            client.DefaultRequestHeaders.Add("Authorization", $"{authHeader}");

            var companyResponse = await client.GetAsync($"/Company/MyCompanies");
            var zoneResponse = await client.GetAsync($"/Zones/Company/{companyId}");

            if (companyResponse.IsSuccessStatusCode && zoneResponse.IsSuccessStatusCode)
            {
                var companyList = _jsonCon.deSerializeAsList<Company>(await companyResponse.Content.ReadAsStringAsync());
                var zoneList = _jsonCon.deSerializeAsList<Zones>(await zoneResponse.Content.ReadAsStringAsync());
                var pdfResponse = pdf.getPdf(companyList, zoneList);

                return File(pdfResponse, "application/octet-stream", "zones-report.pdf");
            }
            else
            {
                return StatusCode(500);
            }
        }

        [HttpGet("Company/Management/OutstandingPayments")]

        public async Task<ActionResult<Summary>> GenerateOutstandingPaymentsReport([FromQuery][Required] string companyId)
        {
            var pdf = new OutstandingPaymentsReport();

            var client = _httpClientFactory.CreateClient("EpumpApi");
            var authHeader = Request.Headers["Authorization"];
            client.DefaultRequestHeaders.Add("Authorization", $"{authHeader}");

            var companyResponse = await client.GetAsync($"/Company/MyCompanies");
            var paymentResponse = await client.GetAsync($"/Branch/AccountBalances?settlement=false&companyId={companyId}");

            if (companyResponse.IsSuccessStatusCode && paymentResponse.IsSuccessStatusCode)
            {
                var companyList = _jsonCon.deSerializeAsList<Company>(await companyResponse.Content.ReadAsStringAsync());
                var paymentList = _jsonCon.deSerializeAsList<OutstandingPayments>(await paymentResponse.Content.ReadAsStringAsync());
                var pdfResponse = pdf.getPdf(companyList, paymentList);

                return pdfResponse;
            }
            else
            {
                return StatusCode(500);
            }
        }

        [HttpGet("Company/Management/CompanyWalletReport")]
        public async Task<ActionResult<Summary>> GenerateCompanyWalletReport([FromQuery][Required] string companyId)
        {
            var pdf = new CompanyWalletReport();

            var client = _httpClientFactory.CreateClient("EpumpApi");
            var authHeader = Request.Headers["Authorization"];
            client.DefaultRequestHeaders.Add("Authorization", $"{authHeader}");

            var companyResponse = await client.GetAsync($"/Company/MyCompanies");
            var detailsResponse = await client.GetAsync($"Company/Wallet/{companyId}");
            var transactionsResponse = await client.GetAsync($"/Company/WalletTransactions/{companyId}");

            if (companyResponse.IsSuccessStatusCode && detailsResponse.IsSuccessStatusCode && transactionsResponse.IsSuccessStatusCode)
            {
                var companyList = _jsonCon.deSerializeAsList<Company>(await companyResponse.Content.ReadAsStringAsync());
                var details = JsonConvert.DeserializeObject<WalletDetails>(await detailsResponse.Content.ReadAsStringAsync());
                var transactionList = _jsonCon.deSerializeAsList<WalletTransactions>(await transactionsResponse.Content.ReadAsStringAsync());
                var pdfResponse = pdf.getPdf(companyList, details, transactionList);

                return pdfResponse;
            }
            else
            {
                return StatusCode(500);
            }
        }

        [HttpGet("Company/Retainerships/RetainershipReport")]
        public async Task<IActionResult> GenerateRetainershipReport([FromQuery][Required] string companyId)
        {
            var pdf = new RetainershipReport();

            var client = _httpClientFactory.CreateClient("EpumpApi");
            var authHeader = Request.Headers["Authorization"];
            client.DefaultRequestHeaders.Add("Authorization", $"{authHeader}");

            var companyResponse = await client.GetAsync($"/Company/MyCompanies");
            var retainershipResponse = await client.GetAsync($"/Company/CorporateCustomers/{companyId}");

            if (companyResponse.IsSuccessStatusCode && retainershipResponse.IsSuccessStatusCode)
            {
                var companyList = _jsonCon.deSerializeAsList<Company>(await companyResponse.Content.ReadAsStringAsync());
                var retainershipList = _jsonCon.deSerializeAsList<Retainership>(await retainershipResponse.Content.ReadAsStringAsync());
                var pdfResponse = pdf.getPdf(companyList, retainershipList);

                return File(pdfResponse, "application/octet-stream", "retainership-report.pdf");
            }
            else
            {
                return StatusCode(500);
            }
        }

        [HttpGet("Company/Retainerships/WalletFundRequestReport")]

        public async Task<ActionResult<Summary>> GenerateWalletFundRequestReport([FromQuery][Required] string companyId, [FromQuery][Required] string status)
        {
            var pdf = new WalletFundRequestReport();

            var client = _httpClientFactory.CreateClient("EpumpApi");
            var authHeader = Request.Headers["Authorization"];
            client.DefaultRequestHeaders.Add("Authorization", $"{authHeader}");

            var companyResponse = await client.GetAsync($"/Company/MyCompanies");
            var bankResponse = await client.GetAsync($"/Wallet/BankTransactions?status={status}&companyId={companyId}");

            if (companyResponse.IsSuccessStatusCode && bankResponse.IsSuccessStatusCode)
            {
                var companyList = _jsonCon.deSerializeAsList<Company>(await companyResponse.Content.ReadAsStringAsync());
                var bankTransactionsList = _jsonCon.deSerializeAsList<BankTransactions>(await bankResponse.Content.ReadAsStringAsync());
                var pdfResponse = pdf.getPdf(companyList, bankTransactionsList);

                return pdfResponse;
            }
            else
            {
                return StatusCode(500);
            }
        }

        [HttpGet("PosTransactionReport")]

        public async Task<ActionResult<Summary>> GeneratePosReport([FromQuery] POSRequestParameters rp)
        {
            var client = _httpClientFactory.CreateClient("EpumpApi");
            var authHeader = Request.Headers["Authorization"];
            client.DefaultRequestHeaders.Add("Authorization", $"{authHeader}");

            HttpResponseMessage companyResponse;
            HttpResponseMessage response;
            Summary pdfResponse;

            if (string.IsNullOrEmpty(rp.branchId) || string.IsNullOrWhiteSpace(rp.branchId))
            {
                var pdf = new CompanyPosTransactions();

                companyResponse = await client.GetAsync($"/Company/MyCompanies");
                response = await client.GetAsync($"/Pos/CompanyTransactions/{rp.companyId}?startDate={rp.startDate}&endDate={rp.endDate}&status={rp.status}");
                if (companyResponse.IsSuccessStatusCode && response.IsSuccessStatusCode)
                {
                    var companyList = _jsonCon.deSerializeAsList<Company>(await companyResponse.Content.ReadAsStringAsync());
                    var posTransactions = _jsonCon.deSerializeAsList<PosTransactions>(await response.Content.ReadAsStringAsync());
                    pdfResponse = pdf.getPdf(posTransactions, companyList);
                }
                else
                {
                    return StatusCode(500);
                }
            }
            else
            {
                companyResponse = await client.GetAsync($"/Company/Branches/{rp.companyId}");
                response = await client.GetAsync($"/Pos/Transactions/{rp.branchId}?date={rp.startDate}&enddate={rp.endDate}&status={rp.status}");

                if (companyResponse.IsSuccessStatusCode && response.IsSuccessStatusCode)
                {
                    var pdf = new BranchPosTransactions();

                    var branchDetails = _jsonCon.deSerializeAsList<BranchDetails>(await companyResponse.Content.ReadAsStringAsync());
                    var posTransactions = _jsonCon.deSerializeAsList<PosTransactions>(await response.Content.ReadAsStringAsync());
                    pdfResponse = pdf.getPdf(rp.branchId, branchDetails, posTransactions);
                }
                else
                {
                    return StatusCode(500);
                }
            }
            // System.Text.Json.JsonSerializer.Serialize(pdfResponse);
            return (pdfResponse);
        }
    }
}
# PowerShell script to generate all DAL classes and Controllers

$masters = @(
    @{Name="CostCenter"; Model="CostCenter"; IdField="CostCenterId"; DALName="CostCenterDAL"; SPPrefix="CostCenter"},
    @{Name="FiscalYear"; Model="FiscalYear"; IdField="FiscalYearId"; DALName="FiscalYearDAL"; SPPrefix="FiscalYear"},
    @{Name="Godown"; Model="Godown"; IdField="GodownId"; DALName="GodownDAL"; SPPrefix="Godown"},
    @{Name="PaymentTerm"; Model="PaymentTerm"; IdField="TermId"; DALName="PaymentTermDAL"; SPPrefix="PaymentTerm"},
    @{Name="Project"; Model="Project"; IdField="ProjectId"; DALName="ProjectDAL"; SPPrefix="Project"},
    @{Name="Role"; Model="Role"; IdField="RoleId"; DALName="RoleDAL"; SPPrefix="Role"},
    @{Name="SAC"; Model="SAC"; IdField="SACId"; DALName="SACDAL"; SPPrefix="SAC"},
    @{Name="Salesperson"; Model="Salesperson"; IdField="SalespersonId"; DALName="SalespersonDAL"; SPPrefix="Salesperson"},
    @{Name="TaxGroup"; Model="TaxGroup"; IdField="TaxGroupId"; DALName="TaxGroupDAL"; SPPrefix="TaxGroup"},
    @{Name="TCS"; Model="TCS"; IdField="TCSId"; DALName="TCSDAL"; SPPrefix="TCS"},
    @{Name="TDS"; Model="TDS"; IdField="TDSId"; DALName="TDSDAL"; SPPrefix="TDS"},
    @{Name="Transporter"; Model="Transporter"; IdField="TransporterId"; DALName="TransporterDAL"; SPPrefix="Transporter"}
)

Write-Host "Generating Controllers for $($masters.Count) masters..."

# Generate Controllers
foreach ($master in $masters) {
    $controllerContent = @"
using Microsoft.AspNetCore.Mvc;
using SoftBook.Data;
using SoftBook.Models;

namespace SoftBook.Controllers
{
    public class $($master.Name)Controller : Controller
    {
        private readonly $($master.DALName) _dal;
        public $($master.Name)Controller($($master.DALName) dal) { _dal = dal; }

        public IActionResult Index() => View(_dal.GetAll());
        public IActionResult Create() => View();

        [HttpPost]
        public IActionResult Create($($master.Model) item)
        {
            if (ModelState.IsValid)
            {
                _dal.Insert(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }

        public IActionResult Edit(int id)
        {
            var item = _dal.GetById(id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost]
        public IActionResult Edit($($master.Model) item)
        {
            if (ModelState.IsValid)
            {
                _dal.Update(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }

        public IActionResult Delete(int id)
        {
            _dal.Delete(id);
            return RedirectToAction("Index");
        }
    }
}
"@
    $controllerContent | Out-File -FilePath "d:\Hetak_Acc\SoftBook\SoftBook\Controllers\$($master.Name)Controller.cs" -Encoding UTF8
    Write-Host "Created controller for $($master.Name)"
}

Write-Host "`nAll controllers created successfully!"
Write-Host "Total controllers created: $($masters.Count)"

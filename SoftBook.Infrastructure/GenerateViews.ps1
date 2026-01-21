# PowerShell script to generate all remaining views for masters

$masters = @(
    @{Name="Tax"; Model="Tax"; IdField="TaxId"; DisplayFields=@("TaxName","RatePercent","TaxType"); FormFields=@("TaxName","RatePercent","TaxType")},
    @{Name="HSN"; Model="HSN"; IdField="HSNId"; DisplayFields=@("HSNCode","Description","TaxRate"); FormFields=@("HSNCode","Description","TaxRate")},
    @{Name="Currency"; Model="Currency"; IdField="CurrencyId"; DisplayFields=@("CurrencyCode","CurrencyName","CurrencySymbol"); FormFields=@("CurrencyCode","CurrencySymbol","CurrencyName","DecimalPlaces","Format")},
    @{Name="PaymentMode"; Model="PaymentMode"; IdField="PaymentModeId"; DisplayFields=@("ModeName","IsDefault"); FormFields=@("ModeName","IsDefault")},
    @{Name="Warehouse"; Model="Warehouse"; IdField="WarehouseId"; DisplayFields=@("WarehouseName","Address"); FormFields=@("WarehouseName","BranchId","Address")},
    @{Name="Branch"; Model="Branch"; IdField="BranchId"; DisplayFields=@("BranchName","City","State","Phone"); FormFields=@("BranchName","Address","City","State","Country","Phone","IsActive")},
    @{Name="Bank"; Model="Bank"; IdField="BankMasterId"; DisplayFields=@("BankName","AccountNumber","IFSC"); FormFields=@("BankName","Branch","AccountNumber","IFSC","MICR")},
    @{Name="AccountHead"; Model="AccountHead"; IdField="AccountId"; DisplayFields=@("AccountName","AccountType","AccountCode"); FormFields=@("AccountType","AccountName","IsSubAccount","ParentAccountId","AccountCode","Description")},
    @{Name="Party"; Model="Party"; IdField="PartyId"; DisplayFields=@("DisplayName","PartyType","Email","PhoneWork"); FormFields=@("PartyType","Salutation","FirstName","LastName","CompanyName","DisplayName","Email","PhoneWork","PhoneMobile","GSTTreatment","PAN")}
)

foreach ($master in $masters) {
    $viewsDir = "d:\Hetak_Acc\SoftBook\SoftBook\Views\$($master.Name)"
    New-Item -ItemType Directory -Force -Path $viewsDir | Out-Null
    
    # Index View
    $indexContent = "@model IEnumerable<SoftBook.Models.$($master.Model)>`r`n@{ ViewData[`"Title`"] = `"$($master.Name)s`"; }`r`n`r`n<h1 class=`"h3 mb-2 text-gray-800`">$($master.Name)s</h1>`r`n<div class=`"card shadow mb-4`">`r`n    <div class=`"card-header py-3`">`r`n        <h6 class=`"m-0 font-weight-bold text-primary`">$($master.Name) List</h6>`r`n    </div>`r`n    <div class=`"card-body`">`r`n        <p><a asp-action=`"Create`" class=`"btn btn-primary`">Create New</a></p>`r`n        <div class=`"table-responsive`">`r`n            <table class=`"table table-bordered`">`r`n                <thead>`r`n                    <tr>`r`n"
    foreach ($field in $master.DisplayFields) {
        $indexContent += "                        <th>$field</th>`r`n"
    }
    $indexContent += "                        <th>Actions</th>`r`n                    </tr>`r`n                </thead>`r`n                <tbody>`r`n                    @foreach (var item in Model)`r`n                    {`r`n                        <tr>`r`n"
    foreach ($field in $master.DisplayFields) {
        $indexContent += "                            <td>@item.$field</td>`r`n"
    }
    $indexContent += "                            <td>`r`n                                <a asp-action=`"Edit`" asp-route-id=`"@item.$($master.IdField)`" class=`"btn btn-sm btn-warning`">Edit</a>`r`n                                <a asp-action=`"Delete`" asp-route-id=`"@item.$($master.IdField)`" class=`"btn btn-sm btn-danger`" onclick=`"return confirm('Are you sure?');`">Delete</a>`r`n                            </td>`r`n                        </tr>`r`n                    }`r`n                </tbody>`r`n            </table>`r`n        </div>`r`n    </div>`r`n</div>"
    $indexContent | Out-File -FilePath "$viewsDir\Index.cshtml" -Encoding UTF8
    
    # Create View
    $createContent = "@model SoftBook.Models.$($master.Model)`r`n@{ ViewData[`"Title`"] = `"Create $($master.Name)`"; }`r`n`r`n<h1 class=`"h3 mb-4 text-gray-800`">Create $($master.Name)</h1>`r`n<div class=`"card shadow mb-4`">`r`n    <div class=`"card-body`">`r`n        <form asp-action=`"Create`">`r`n"
    foreach ($field in $master.FormFields) {
        $createContent += "            <div class=`"form-group`">`r`n                <label asp-for=`"$field`"></label>`r`n                <input asp-for=`"$field`" class=`"form-control`" />`r`n            </div>`r`n"
    }
    $createContent += "            <div class=`"form-group mt-3`">`r`n                <input type=`"submit`" value=`"Create`" class=`"btn btn-primary`" />`r`n                <a asp-action=`"Index`" class=`"btn btn-secondary`">Back</a>`r`n            </div>`r`n        </form>`r`n    </div>`r`n</div>"
    $createContent | Out-File -FilePath "$viewsDir\Create.cshtml" -Encoding UTF8
    
    # Edit View
    $editContent = "@model SoftBook.Models.$($master.Model)`r`n@{ ViewData[`"Title`"] = `"Edit $($master.Name)`"; }`r`n`r`n<h1 class=`"h3 mb-4 text-gray-800`">Edit $($master.Name)</h1>`r`n<div class=`"card shadow mb-4`">`r`n    <div class=`"card-body`">`r`n        <form asp-action=`"Edit`">`r`n            <input type=`"hidden`" asp-for=`"$($master.IdField)`" />`r`n"
    foreach ($field in $master.FormFields) {
        $editContent += "            <div class=`"form-group`">`r`n                <label asp-for=`"$field`"></label>`r`n                <input asp-for=`"$field`" class=`"form-control`" />`r`n            </div>`r`n"
    }
    $editContent += "            <div class=`"form-group mt-3`">`r`n                <input type=`"submit`" value=`"Save`" class=`"btn btn-primary`" />`r`n                <a asp-action=`"Index`" class=`"btn btn-secondary`">Back</a>`r`n            </div>`r`n        </form>`r`n    </div>`r`n</div>"
    $editContent | Out-File -FilePath "$viewsDir\Edit.cshtml" -Encoding UTF8
    
    Write-Host "Created views for $($master.Name)"
}

Write-Host "All views created successfully!"

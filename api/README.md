# README

## EPUMP Report Generator
This generates reports across the platform. This README explains how it works.

## Prequisites
JSON.NET  
iText7  
Net 5.0  

## Report Structure
Each report accepts List and string parameters. For company or branch details, the table data and a time period, e.g
` public byte[] getPdf(string branchId, List<BranchDetails> branchDetails, List<PosTransactions> posTransactionsList) `

They all return an array of bytes.

## Portrait Reports
These are the ones that inherit from `PortraitTemplate`.

PdfWriter writes to  a memory stream, reads in the template from a specified file via PdfReader,
then creates a new document using the both.

The document has a pageSize and margins(which prevent items from overlapping).
The background Image handler adds a the set background image to each page. *It should not be touched, it's perfect as is.*

The PdfTextFormField's are used to display important info. The branch or company details and also a summary for tables with lots of numbers.
To adjust their position on the page tweak the numbers here:
`new Rectangle(455, 1005, 200, 15)`
You can also use `.setBorderColor(ColorConstants.BLACK)` to better understand their position on the page.


`Table dataTable = new Table(float[], bool largeTable)`  
Here, you set the number of rows in the table and their relative widths. You can also just use an integer value, but that would make each row equal. If largeTable is true, the table takes up more space on the page, which is intended.
**Make sure the number of items being added match the number of rows in the table.**

*Inserting into the table*  
`dataTable.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph("ITEM TO BE ADDED"))).SetFontSize(10).SetTextAlignment(TextAlignment.CENTER);`

1. First, add a new cell to the table
1. Set the cell border to none
1. Add a new paragraph
1. Set Paragraph properties

**NOTE** Background color must be set for the cell.

After branch or company details are set:
* The table is added to the document
* The footer is added (THIS MUST BE DONE AFTER THE TABLE, FOR PAGES TO COUNT CORRECTLY)
* The fields are flattened to prevent modification.
* The document is flushed, and closed. Note that the document must be flushed and closed to display properly.
* Then a byte array is returned.

## HOW TO TEST

The [endpoints](https://pdf.epump.club/swagger/index.html) are hosted here

Using Postman, add query parameters as required, then provide authorization.

Dates should be in this format: June 2, 2020.  
Any unnecessary parameters must be left empty, like so: ""

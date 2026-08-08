using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cor.CRM.Migrations
{
    /// <inheritdoc />
    public partial class AddContractEntitiesjhhjhddsdsd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Property",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Address = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    City = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    State = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    PostalCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Country = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Latitude = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Longitude = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Bedrooms = table.Column<int>(type: "integer", nullable: true),
                    Bathrooms = table.Column<int>(type: "integer", nullable: true),
                    HalfBathrooms = table.Column<int>(type: "integer", nullable: true),
                    LandSize = table.Column<decimal>(type: "numeric(12,2)", nullable: true),
                    BuildingSize = table.Column<decimal>(type: "numeric(12,2)", nullable: true),
                    YearBuilt = table.Column<int>(type: "integer", nullable: true),
                    Price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    PricePerSquareFoot = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    ListedPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    SoldPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: true),
                    ListingAgentId = table.Column<Guid>(type: "uuid", nullable: true),
                    BuyingAgentId = table.Column<Guid>(type: "uuid", nullable: true),
                    FeaturesJson = table.Column<string>(type: "text", nullable: true),
                    MainImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ImagesJson = table.Column<string>(type: "text", nullable: true),
                    VirtualTourUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    VideoUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    DocumentsJson = table.Column<string>(type: "text", nullable: true),
                    ListingDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SoldDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RentedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    OffMarketDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    MarketingDescription = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsFeatured = table.Column<bool>(type: "boolean", nullable: false),
                    IsPublished = table.Column<bool>(type: "boolean", nullable: false),
                    ViewCount = table.Column<int>(type: "integer", nullable: false),
                    InquiryCount = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    UpdatedByUserName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Property", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Property_Customers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Customers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Property_LocalEmployees_BuyingAgentId",
                        column: x => x.BuyingAgentId,
                        principalTable: "LocalEmployees",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Property_LocalEmployees_ListingAgentId",
                        column: x => x.ListingAgentId,
                        principalTable: "LocalEmployees",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PropertyInquiry",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PropertyId = table.Column<Guid>(type: "uuid", nullable: false),
                    LeadId = table.Column<Guid>(type: "uuid", nullable: true),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: true),
                    Message = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ContactedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ViewedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    UpdatedByUserName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyInquiry", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PropertyInquiry_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PropertyInquiry_Leads_LeadId",
                        column: x => x.LeadId,
                        principalTable: "Leads",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PropertyInquiry_Property_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "Property",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PropertyViewings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PropertyId = table.Column<Guid>(type: "uuid", nullable: false),
                    LeadId = table.Column<Guid>(type: "uuid", nullable: true),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: true),
                    AgentId = table.Column<Guid>(type: "uuid", nullable: true),
                    ViewingDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Feedback = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Rating = table.Column<int>(type: "integer", nullable: true),
                    IsVirtual = table.Column<bool>(type: "boolean", nullable: false),
                    IsFollowUp = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    UpdatedByUserName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyViewings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PropertyViewings_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PropertyViewings_Leads_LeadId",
                        column: x => x.LeadId,
                        principalTable: "Leads",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PropertyViewings_LocalEmployees_AgentId",
                        column: x => x.AgentId,
                        principalTable: "LocalEmployees",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PropertyViewings_Property_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "Property",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RealEstateTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TransactionNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PropertyId = table.Column<Guid>(type: "uuid", nullable: false),
                    BuyerId = table.Column<Guid>(type: "uuid", nullable: false),
                    SellerId = table.Column<Guid>(type: "uuid", nullable: false),
                    BuyerAgentId = table.Column<Guid>(type: "uuid", nullable: true),
                    SellerAgentId = table.Column<Guid>(type: "uuid", nullable: true),
                    SalePrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    DepositAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    CommissionAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    BuyerCommission = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    SellerCommission = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    OfferDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AcceptanceDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ClosingDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PossessionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    DocumentsJson = table.Column<string>(type: "text", nullable: true),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    QuoteId = table.Column<Guid>(type: "uuid", nullable: true),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: true),
                    ContractId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    UpdatedByUserName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RealEstateTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RealEstateTransactions_Contracts_ContractId",
                        column: x => x.ContractId,
                        principalTable: "Contracts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RealEstateTransactions_Customers_BuyerId",
                        column: x => x.BuyerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RealEstateTransactions_Customers_SellerId",
                        column: x => x.SellerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RealEstateTransactions_LocalEmployees_BuyerAgentId",
                        column: x => x.BuyerAgentId,
                        principalTable: "LocalEmployees",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RealEstateTransactions_LocalEmployees_SellerAgentId",
                        column: x => x.SellerAgentId,
                        principalTable: "LocalEmployees",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RealEstateTransactions_Property_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "Property",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RealEstateTransactions_Quotes_QuoteId",
                        column: x => x.QuoteId,
                        principalTable: "Quotes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RealEstateTransactions_SalesOrders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "SalesOrders",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Commissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TransactionId = table.Column<Guid>(type: "uuid", nullable: false),
                    AgentId = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Percentage = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsBuyerAgent = table.Column<bool>(type: "boolean", nullable: false),
                    IsSellerAgent = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    UpdatedByUserName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Commissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Commissions_LocalEmployees_AgentId",
                        column: x => x.AgentId,
                        principalTable: "LocalEmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Commissions_RealEstateTransactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "RealEstateTransactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Commissions_AgentId",
                table: "Commissions",
                column: "AgentId");

            migrationBuilder.CreateIndex(
                name: "IX_Commissions_TransactionId",
                table: "Commissions",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_Property_BuyingAgentId",
                table: "Property",
                column: "BuyingAgentId");

            migrationBuilder.CreateIndex(
                name: "IX_Property_ListingAgentId",
                table: "Property",
                column: "ListingAgentId");

            migrationBuilder.CreateIndex(
                name: "IX_Property_OwnerId",
                table: "Property",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyInquiry_CustomerId",
                table: "PropertyInquiry",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyInquiry_LeadId",
                table: "PropertyInquiry",
                column: "LeadId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyInquiry_PropertyId",
                table: "PropertyInquiry",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyViewings_AgentId",
                table: "PropertyViewings",
                column: "AgentId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyViewings_CustomerId",
                table: "PropertyViewings",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyViewings_LeadId",
                table: "PropertyViewings",
                column: "LeadId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyViewings_PropertyId",
                table: "PropertyViewings",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_RealEstateTransactions_BuyerAgentId",
                table: "RealEstateTransactions",
                column: "BuyerAgentId");

            migrationBuilder.CreateIndex(
                name: "IX_RealEstateTransactions_BuyerId",
                table: "RealEstateTransactions",
                column: "BuyerId");

            migrationBuilder.CreateIndex(
                name: "IX_RealEstateTransactions_ContractId",
                table: "RealEstateTransactions",
                column: "ContractId");

            migrationBuilder.CreateIndex(
                name: "IX_RealEstateTransactions_OrderId",
                table: "RealEstateTransactions",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_RealEstateTransactions_PropertyId",
                table: "RealEstateTransactions",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_RealEstateTransactions_QuoteId",
                table: "RealEstateTransactions",
                column: "QuoteId");

            migrationBuilder.CreateIndex(
                name: "IX_RealEstateTransactions_SellerAgentId",
                table: "RealEstateTransactions",
                column: "SellerAgentId");

            migrationBuilder.CreateIndex(
                name: "IX_RealEstateTransactions_SellerId",
                table: "RealEstateTransactions",
                column: "SellerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Commissions");

            migrationBuilder.DropTable(
                name: "PropertyInquiry");

            migrationBuilder.DropTable(
                name: "PropertyViewings");

            migrationBuilder.DropTable(
                name: "RealEstateTransactions");

            migrationBuilder.DropTable(
                name: "Property");
        }
    }
}

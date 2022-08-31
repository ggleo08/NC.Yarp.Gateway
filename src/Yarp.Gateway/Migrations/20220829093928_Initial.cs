using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Yarp.Gateway.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "YarpClusters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClusterId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LoadBalancingPolicy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YarpClusters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "YarpSessionAffinityOptionSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YarpSessionAffinityOptionSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "YarpDestinations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Health = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClusterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YarpDestinations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_YarpDestinations_YarpClusters_ClusterId",
                        column: x => x.ClusterId,
                        principalTable: "YarpClusters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "YarpHealthCheckOptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AvailableDestinationsPolicy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClusterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YarpHealthCheckOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_YarpHealthCheckOptions_YarpClusters_ClusterId",
                        column: x => x.ClusterId,
                        principalTable: "YarpClusters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "YarpProxyHttpClientOptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SslProtocols = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DangerousAcceptAnyServerCertificate = table.Column<bool>(type: "bit", nullable: true),
                    MaxConnectionsPerServer = table.Column<int>(type: "int", nullable: true),
                    EnableMultipleHttp2Connections = table.Column<bool>(type: "bit", nullable: true),
                    RequestHeaderEncoding = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClusterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YarpProxyHttpClientOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_YarpProxyHttpClientOptions_YarpClusters_ClusterId",
                        column: x => x.ClusterId,
                        principalTable: "YarpClusters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "YarpRequestProxyOptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActivityTimeout = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Version = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VersionPolicy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AllowResponseBuffering = table.Column<bool>(type: "bit", nullable: true),
                    ClusterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YarpRequestProxyOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_YarpRequestProxyOptions_YarpClusters_ClusterId",
                        column: x => x.ClusterId,
                        principalTable: "YarpClusters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "YarpRoutes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RouteId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Order = table.Column<int>(type: "int", nullable: true),
                    ClusterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AuthorizationPolicy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CorsPolicy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YarpRoutes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_YarpRoutes_YarpClusters_ClusterId",
                        column: x => x.ClusterId,
                        principalTable: "YarpClusters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "YarpSessionAffinityOptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Enabled = table.Column<bool>(type: "bit", nullable: true),
                    Policy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FailurePolicy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AffinityKeyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClusterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YarpSessionAffinityOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_YarpSessionAffinityOptions_YarpClusters_ClusterId",
                        column: x => x.ClusterId,
                        principalTable: "YarpClusters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "YarpActiveHealthCheckOptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Enabled = table.Column<bool>(type: "bit", nullable: true),
                    Interval = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Timeout = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Policy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Path = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HealthCheckOptionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YarpActiveHealthCheckOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_YarpActiveHealthCheckOptions_YarpHealthCheckOptions_HealthCheckOptionId",
                        column: x => x.HealthCheckOptionId,
                        principalTable: "YarpHealthCheckOptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "YarpPassiveHealthCheckOptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Enabled = table.Column<bool>(type: "bit", nullable: true),
                    Policy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReactivationPeriod = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HealthCheckOptionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YarpPassiveHealthCheckOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_YarpPassiveHealthCheckOptions_YarpHealthCheckOptions_HealthCheckOptionId",
                        column: x => x.HealthCheckOptionId,
                        principalTable: "YarpHealthCheckOptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "YarpWebProxyConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BypassOnLocal = table.Column<bool>(type: "bit", nullable: true),
                    UseDefaultCredentials = table.Column<bool>(type: "bit", nullable: true),
                    HttpClientOptionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YarpWebProxyConfig", x => x.Id);
                    table.ForeignKey(
                        name: "FK_YarpWebProxyConfig_YarpProxyHttpClientOptions_HttpClientOptionId",
                        column: x => x.HttpClientOptionId,
                        principalTable: "YarpProxyHttpClientOptions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "YarpMatches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Methods = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Hosts = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Path = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RouteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YarpMatches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_YarpMatches_YarpRoutes_RouteId",
                        column: x => x.RouteId,
                        principalTable: "YarpRoutes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "YarpMetadatas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RouteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClusterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DestinationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    YarpClusterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    YarpDestinationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    YarpRouteId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Key = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YarpMetadatas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_YarpMetadatas_YarpClusters_ClusterId",
                        column: x => x.ClusterId,
                        principalTable: "YarpClusters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_YarpMetadatas_YarpClusters_YarpClusterId",
                        column: x => x.YarpClusterId,
                        principalTable: "YarpClusters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_YarpMetadatas_YarpDestinations_DestinationId",
                        column: x => x.DestinationId,
                        principalTable: "YarpDestinations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_YarpMetadatas_YarpDestinations_YarpDestinationId",
                        column: x => x.YarpDestinationId,
                        principalTable: "YarpDestinations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_YarpMetadatas_YarpRoutes_RouteId",
                        column: x => x.RouteId,
                        principalTable: "YarpRoutes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_YarpMetadatas_YarpRoutes_YarpRouteId",
                        column: x => x.YarpRouteId,
                        principalTable: "YarpRoutes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "YarpTransforms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    RouteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YarpTransforms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_YarpTransforms_YarpRoutes_RouteId",
                        column: x => x.RouteId,
                        principalTable: "YarpRoutes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "YarpSessionAffinityCookie",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Path = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Domain = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HttpOnly = table.Column<bool>(type: "bit", nullable: true),
                    SecurePolicy = table.Column<int>(type: "int", nullable: true),
                    SameSite = table.Column<int>(type: "int", nullable: true),
                    Expiration = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaxAge = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsEssential = table.Column<bool>(type: "bit", nullable: true),
                    SessionAffinityConfigId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YarpSessionAffinityCookie", x => x.Id);
                    table.ForeignKey(
                        name: "FK_YarpSessionAffinityCookie_YarpSessionAffinityOptions_SessionAffinityConfigId",
                        column: x => x.SessionAffinityConfigId,
                        principalTable: "YarpSessionAffinityOptions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "YarpRouteHeaders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Values = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Mode = table.Column<int>(type: "int", nullable: false),
                    IsCaseSensitive = table.Column<bool>(type: "bit", nullable: false),
                    MatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YarpRouteHeaders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_YarpRouteHeaders_YarpMatches_MatchId",
                        column: x => x.MatchId,
                        principalTable: "YarpMatches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "YarpRouteQueryParameter",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Values = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Mode = table.Column<int>(type: "int", nullable: false),
                    IsCaseSensitive = table.Column<bool>(type: "bit", nullable: false),
                    MatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YarpRouteQueryParameter", x => x.Id);
                    table.ForeignKey(
                        name: "FK_YarpRouteQueryParameter_YarpMatches_MatchId",
                        column: x => x.MatchId,
                        principalTable: "YarpMatches",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "YarpClusters",
                columns: new[] { "Id", "ClusterId", "LoadBalancingPolicy" },
                values: new object[] { new Guid("bdfdc599-5306-45cf-8ad2-8b995e84801f"), "cluster1", null });

            migrationBuilder.InsertData(
                table: "YarpClusters",
                columns: new[] { "Id", "ClusterId", "LoadBalancingPolicy" },
                values: new object[] { new Guid("f0fa724b-d1c6-4845-ac70-5ad59a860403"), "cluster2", null });

            migrationBuilder.InsertData(
                table: "YarpDestinations",
                columns: new[] { "Id", "Address", "ClusterId", "Health", "Name" },
                values: new object[,]
                {
                    { new Guid("1ede2881-92c4-4689-befc-2ef019c6ef60"), "http://localhost:5251", new Guid("bdfdc599-5306-45cf-8ad2-8b995e84801f"), null, "Cluster1/Destination1" },
                    { new Guid("20f20b36-9956-427c-b6b6-5b8cf711ca20"), "http://localhost:5252", new Guid("f0fa724b-d1c6-4845-ac70-5ad59a860403"), null, "Cluster2/Destination2" }
                });

            migrationBuilder.InsertData(
                table: "YarpRoutes",
                columns: new[] { "Id", "AuthorizationPolicy", "ClusterId", "CorsPolicy", "Order", "RouteId" },
                values: new object[,]
                {
                    { new Guid("1a5081de-6d33-468c-92e0-35c9522e01f4"), null, new Guid("bdfdc599-5306-45cf-8ad2-8b995e84801f"), null, null, "route2" },
                    { new Guid("32e7f141-fa42-4cf0-ad09-9c50a1fe9e3b"), "Default", new Guid("bdfdc599-5306-45cf-8ad2-8b995e84801f"), null, null, "route1" }
                });

            migrationBuilder.InsertData(
                table: "YarpMatches",
                columns: new[] { "Id", "Hosts", "Methods", "Path", "RouteId" },
                values: new object[,]
                {
                    { new Guid("58d29d21-e1ec-4b25-8cfa-c06d164c6593"), null, null, "/api/first/{**catch-all}", new Guid("32e7f141-fa42-4cf0-ad09-9c50a1fe9e3b") },
                    { new Guid("a15addda-c363-41df-90ff-20d7a35bf17b"), null, null, "/api/second/{**catch-all}", new Guid("1a5081de-6d33-468c-92e0-35c9522e01f4") }
                });

            migrationBuilder.InsertData(
                table: "YarpTransforms",
                columns: new[] { "Id", "Key", "RouteId", "Type", "Value" },
                values: new object[,]
                {
                    { new Guid("54475d14-1ef8-4bed-92b4-8bcb17505066"), null, new Guid("32e7f141-fa42-4cf0-ad09-9c50a1fe9e3b"), 4, "/api/{**catch-all}" },
                    { new Guid("f994ad38-8eee-43f9-8001-6f418fd84455"), null, new Guid("32e7f141-fa42-4cf0-ad09-9c50a1fe9e3b"), 4, "/api/{**catch-all}" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_YarpActiveHealthCheckOptions_HealthCheckOptionId",
                table: "YarpActiveHealthCheckOptions",
                column: "HealthCheckOptionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_YarpDestinations_ClusterId",
                table: "YarpDestinations",
                column: "ClusterId");

            migrationBuilder.CreateIndex(
                name: "IX_YarpHealthCheckOptions_ClusterId",
                table: "YarpHealthCheckOptions",
                column: "ClusterId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_YarpMatches_RouteId",
                table: "YarpMatches",
                column: "RouteId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_YarpMetadatas_ClusterId",
                table: "YarpMetadatas",
                column: "ClusterId");

            migrationBuilder.CreateIndex(
                name: "IX_YarpMetadatas_DestinationId",
                table: "YarpMetadatas",
                column: "DestinationId");

            migrationBuilder.CreateIndex(
                name: "IX_YarpMetadatas_RouteId",
                table: "YarpMetadatas",
                column: "RouteId");

            migrationBuilder.CreateIndex(
                name: "IX_YarpMetadatas_YarpClusterId",
                table: "YarpMetadatas",
                column: "YarpClusterId");

            migrationBuilder.CreateIndex(
                name: "IX_YarpMetadatas_YarpDestinationId",
                table: "YarpMetadatas",
                column: "YarpDestinationId");

            migrationBuilder.CreateIndex(
                name: "IX_YarpMetadatas_YarpRouteId",
                table: "YarpMetadatas",
                column: "YarpRouteId");

            migrationBuilder.CreateIndex(
                name: "IX_YarpPassiveHealthCheckOptions_HealthCheckOptionId",
                table: "YarpPassiveHealthCheckOptions",
                column: "HealthCheckOptionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_YarpProxyHttpClientOptions_ClusterId",
                table: "YarpProxyHttpClientOptions",
                column: "ClusterId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_YarpRequestProxyOptions_ClusterId",
                table: "YarpRequestProxyOptions",
                column: "ClusterId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_YarpRouteHeaders_MatchId",
                table: "YarpRouteHeaders",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_YarpRouteQueryParameter_MatchId",
                table: "YarpRouteQueryParameter",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_YarpRoutes_ClusterId",
                table: "YarpRoutes",
                column: "ClusterId");

            migrationBuilder.CreateIndex(
                name: "IX_YarpSessionAffinityCookie_SessionAffinityConfigId",
                table: "YarpSessionAffinityCookie",
                column: "SessionAffinityConfigId",
                unique: true,
                filter: "[SessionAffinityConfigId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_YarpSessionAffinityOptions_ClusterId",
                table: "YarpSessionAffinityOptions",
                column: "ClusterId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_YarpTransforms_RouteId",
                table: "YarpTransforms",
                column: "RouteId");

            migrationBuilder.CreateIndex(
                name: "IX_YarpWebProxyConfig_HttpClientOptionId",
                table: "YarpWebProxyConfig",
                column: "HttpClientOptionId",
                unique: true,
                filter: "[HttpClientOptionId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "YarpActiveHealthCheckOptions");

            migrationBuilder.DropTable(
                name: "YarpMetadatas");

            migrationBuilder.DropTable(
                name: "YarpPassiveHealthCheckOptions");

            migrationBuilder.DropTable(
                name: "YarpRequestProxyOptions");

            migrationBuilder.DropTable(
                name: "YarpRouteHeaders");

            migrationBuilder.DropTable(
                name: "YarpRouteQueryParameter");

            migrationBuilder.DropTable(
                name: "YarpSessionAffinityCookie");

            migrationBuilder.DropTable(
                name: "YarpSessionAffinityOptionSettings");

            migrationBuilder.DropTable(
                name: "YarpTransforms");

            migrationBuilder.DropTable(
                name: "YarpWebProxyConfig");

            migrationBuilder.DropTable(
                name: "YarpDestinations");

            migrationBuilder.DropTable(
                name: "YarpHealthCheckOptions");

            migrationBuilder.DropTable(
                name: "YarpMatches");

            migrationBuilder.DropTable(
                name: "YarpSessionAffinityOptions");

            migrationBuilder.DropTable(
                name: "YarpProxyHttpClientOptions");

            migrationBuilder.DropTable(
                name: "YarpRoutes");

            migrationBuilder.DropTable(
                name: "YarpClusters");
        }
    }
}

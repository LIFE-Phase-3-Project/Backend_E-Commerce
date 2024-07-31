using Domain.DTOs.Pagination;
using Domain.DTOs.Product;
using Nest;

namespace Application.Services.Search
{
    public class SearchService : ISearchService
    {
        private readonly IElasticClient _elasticsearchClient;

        public SearchService(IElasticClient elasticsearchClient)
        {
            _elasticsearchClient = elasticsearchClient;
        }

        public async Task<bool> IndexProductAsync(ProductIndexDto product)
        {
            var response = await _elasticsearchClient.IndexDocumentAsync(product);
            return response.IsValid;
        }
        // method to remove a product from the index
        public async Task<bool> DeleteProductFromIndexAsync(int productId)
        {
            var response = await _elasticsearchClient.DeleteByQueryAsync<ProductIndexDto>(q => q
                .Query(q => q
                    .Match(m => m
                        .Field(f => f.Id)
                        .Query(productId.ToString())
                    )
                )
            );
            return response.IsValid;
        }


        public async Task<IEnumerable<ProductIndexDto>> SearchProductsAsYouType(string query)
        {
            var searchResponse = await _elasticsearchClient.SearchAsync<ProductIndexDto>(s => s
                 .Size(10) 
                 .Query(q => q
                     .MatchPhrasePrefix(m => m
                         .Field(f => f.Title)
                         .Query(query.ToLower())
                     ) || q
                     .MatchPhrasePrefix(m => m
                         .Field(f => f.Description)
                         .Query(query.ToLower())
                     )
                 )
            );

            return searchResponse.Documents;
        }
        public async Task<PaginatedInfo<ProductIndexDto>> SearchProductsAsync(ProductFilterModel filters, int page, int pageSize)
        {
            var mustQueries = new List<QueryContainer>();

            if (!string.IsNullOrEmpty(filters.SearchTerm))
            {
                var lowercaseTerm = filters.SearchTerm.ToLower();
               mustQueries.Add(new QueryContainer(new WildcardQuery
               {
                   Field = "title",
                   Value = $"*{lowercaseTerm}*"
               }) || new QueryContainer(new WildcardQuery
               {
                   Field = "description",
                   Value = $"*{lowercaseTerm}*"
               }));

            }
            if (filters.StockMin.HasValue)
            {
                mustQueries.Add(new NumericRangeQuery
                {
                    Field = "stock",
                    GreaterThanOrEqualTo = filters.StockMin
                });
            }
            if (filters.StockMax.HasValue)
            {
                mustQueries.Add(new NumericRangeQuery
                {
                    Field = "stock",
                    LessThanOrEqualTo =  filters.StockMax
                });
            }
            if (filters.MinPrice.HasValue)
            {
                mustQueries.Add(new NumericRangeQuery
                {
                    Field = "price",
                    GreaterThanOrEqualTo = (double) filters.MinPrice                
                });
            }
            if (filters.MaxPrice.HasValue)
            {
                mustQueries.Add(new NumericRangeQuery
                {
                    Field = "price",
                    LessThanOrEqualTo = (double) filters.MaxPrice
                });
            }

            if (!string.IsNullOrEmpty(filters.Color))
            {
                mustQueries.Add(new TermQuery
                {
                    Field = "color",
                    Value = filters.Color
                });
            }

            if (filters.CategoryId.HasValue)
            {
                mustQueries.Add(new TermQuery
                {
                    Field = "categoryId",
                    Value = filters.CategoryId
                });
            }

            if (filters.SubCategoryId.HasValue)
            {
                mustQueries.Add(new TermQuery
                {
                    Field = "subCategoryId",
                    Value = filters.SubCategoryId
                });
            }
            var sortDescriptor = new SortDescriptor<ProductIndexDto>();

            if (!string.IsNullOrEmpty(filters.SortOrder))
            {
                if (filters.SortOrder == "z-a")
                {
                    sortDescriptor.Ascending(f => f.Title.Suffix("keyword"));
                }
                else if (filters.SortOrder == "a-z")
                {
                    sortDescriptor.Descending(f => f.Title.Suffix("keyword"));
                }
                else if (filters.SortOrder == "price-asc")
                {
                    sortDescriptor.Ascending(f => f.Price);
                }
                else if (filters.SortOrder == "price-desc")
                {
                    sortDescriptor.Descending(f => f.Price);
                }
            }
            if (mustQueries.Count == 0)
            {
                mustQueries.Add(new MatchAllQuery());
            }
            var searchResponse = await _elasticsearchClient.SearchAsync<ProductIndexDto>(s => s
                .From((page - 1) * pageSize)
                .Size(pageSize)
                .Query(q => q.Bool(b => b.Must(mustQueries.ToArray())))
                .Sort(sort => sortDescriptor)
);

            var paginatedInfo = new PaginatedInfo<ProductIndexDto>
            {
                Items = searchResponse.Documents.ToList(),
                TotalCount = (int)searchResponse.Total,
                Page = page,
                PageSize = pageSize
            };
            return paginatedInfo;
        }

    }
}
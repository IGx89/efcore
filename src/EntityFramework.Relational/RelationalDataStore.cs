// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Data.Entity.ChangeTracking.Internal;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Query;
using Microsoft.Data.Entity.Relational.Query;
using Microsoft.Data.Entity.Relational.Query.Methods;
using Microsoft.Data.Entity.Relational.Update;
using Microsoft.Data.Entity.Storage;
using Microsoft.Data.Entity.Utilities;
using Microsoft.Framework.Logging;
using Remotion.Linq;

namespace Microsoft.Data.Entity.Relational
{
    public abstract class RelationalDataStore : DataStore
    {
        private readonly CommandBatchPreparer _batchPreparer;
        private readonly BatchExecutor _batchExecutor;
        private readonly RelationalConnection _connection;
        private readonly IDbContextOptions _options;

        /// <summary>
        ///     This constructor is intended only for use when creating test doubles that will override members
        ///     with mocked or faked behavior. Use of this constructor for other purposes may result in unexpected
        ///     behavior including but not limited to throwing <see cref="NullReferenceException" />.
        /// </summary>
        protected RelationalDataStore()
        {
        }

        protected RelationalDataStore(
            [NotNull] StateManager stateManager,
            [NotNull] DbContextService<IModel> model,
            [NotNull] EntityKeyFactorySource entityKeyFactorySource,
            [NotNull] EntityMaterializerSource entityMaterializerSource,
            [NotNull] ClrCollectionAccessorSource collectionAccessorSource,
            [NotNull] ClrPropertySetterSource propertySetterSource,
            [NotNull] RelationalConnection connection,
            [NotNull] CommandBatchPreparer batchPreparer,
            [NotNull] BatchExecutor batchExecutor,
            [NotNull] DbContextService<IDbContextOptions> options,
            [NotNull] ILoggerFactory loggerFactory)
            : base(
                Check.NotNull(stateManager, "stateManager"),
                Check.NotNull(model, "model"),
                Check.NotNull(entityKeyFactorySource, "entityKeyFactorySource"),
                Check.NotNull(entityMaterializerSource, "entityMaterializerSource"),
                Check.NotNull(collectionAccessorSource, "collectionAccessorSource"),
                Check.NotNull(propertySetterSource, "propertySetterSource"),
                Check.NotNull(loggerFactory, "loggerFactory"))
        {
            Check.NotNull(connection, nameof(connection));
            Check.NotNull(batchPreparer, nameof(batchPreparer));
            Check.NotNull(batchExecutor, nameof(batchExecutor));
            Check.NotNull(options, nameof(options));

            _batchPreparer = batchPreparer;
            _batchExecutor = batchExecutor;
            _connection = connection;
            _options = options.Service;
        }

        protected virtual RelationalValueReaderFactory ValueReaderFactory => new RelationalTypedValueReaderFactory();

        public virtual IDbContextOptions DbContextOptions => _options;

        public override int SaveChanges(
            IReadOnlyList<InternalEntityEntry> entries)
        {
            Check.NotNull(entries, nameof(entries));

            var commandBatches = _batchPreparer.BatchCommands(entries, _options);

            return _batchExecutor.Execute(commandBatches, _connection);
        }

        public override Task<int> SaveChangesAsync(
            IReadOnlyList<InternalEntityEntry> entries,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Check.NotNull(entries, nameof(entries));

            var commandBatches = _batchPreparer.BatchCommands(entries, _options);

            return _batchExecutor.ExecuteAsync(commandBatches, _connection, cancellationToken);
        }

        public override Func<QueryContext, IEnumerable<TResult>> CompileQuery<TResult>(QueryModel queryModel)
        {
            Check.NotNull(queryModel, nameof(queryModel));

            var queryCompilationContext
                = CreateQueryCompilationContext(
                    new LinqOperatorProvider(),
                    new RelationalResultOperatorHandler(),
                    new QueryMethodProvider(),
                    new CompositeMethodCallTranslator());

            return queryCompilationContext
                .CreateQueryModelVisitor()
                .CreateQueryExecutor<TResult>(queryModel);
        }

        public override Func<QueryContext, IAsyncEnumerable<TResult>> CompileAsyncQuery<TResult>(QueryModel queryModel)
        {
            var queryCompilationContext
                = CreateQueryCompilationContext(
                    new AsyncLinqOperatorProvider(),
                    new RelationalResultOperatorHandler(),
                    new AsyncQueryMethodProvider(),
                    new CompositeMethodCallTranslator());

            return queryCompilationContext
                .CreateQueryModelVisitor()
                .CreateAsyncQueryExecutor<TResult>(queryModel);
        }

        public override QueryContext CreateQueryContext()
        {
            return new RelationalQueryContext(Logger, CreateQueryBuffer(), _connection, ValueReaderFactory);
        }

        protected virtual RelationalQueryCompilationContext CreateQueryCompilationContext(
            [NotNull] ILinqOperatorProvider linqOperatorProvider,
            [NotNull] IResultOperatorHandler resultOperatorHandler,
            [NotNull] IQueryMethodProvider queryMethodProvider,
            [NotNull] IMethodCallTranslator methodCallTranslator)
        {
            Check.NotNull(linqOperatorProvider, nameof(linqOperatorProvider));
            Check.NotNull(resultOperatorHandler, nameof(resultOperatorHandler));
            Check.NotNull(queryMethodProvider, nameof(queryMethodProvider));
            Check.NotNull(methodCallTranslator, nameof(methodCallTranslator));

            return new RelationalQueryCompilationContext(
                Model,
                Logger,
                linqOperatorProvider,
                resultOperatorHandler,
                EntityMaterializerSource,
                EntityKeyFactorySource,
                queryMethodProvider,
                methodCallTranslator);
        }
    }
}

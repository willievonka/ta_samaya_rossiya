// @ts-check
const eslint = require("@eslint/js");
const tseslint = require("typescript-eslint");
const angular = require("angular-eslint");
const jsdoc = require("eslint-plugin-jsdoc");
const angularEslintPlugin = require("@angular-eslint/eslint-plugin");
const unusedImports = require("eslint-plugin-unused-imports");
const filenameRules = require("eslint-plugin-filename-rules");

module.exports = tseslint.config(
    {
        files: ["**/*.ts"],
        extends: [
            eslint.configs.recommended,
            ...tseslint.configs.recommended,
            ...tseslint.configs.stylistic,
            ...angular.configs.tsRecommended,
        ],
        plugins: {
            "@typescript-eslint": tseslint.plugin,
            jsdoc,
            '@angular-eslint': angularEslintPlugin,
            'unused-imports': unusedImports,
            'filename-rules': filenameRules,
        },
        processor: angular.processInlineTemplates,
        rules: {
            "@typescript-eslint/no-inferrable-types": "off",
            "@angular-eslint/directive-selector": [
                "error",
                {
                    type: "attribute",
                    prefix: "app",
                    style: "camelCase",
                },
            ],
            "@angular-eslint/component-selector": [
                "error",
                {
                    type: "element",
                    prefix: "app",
                    style: "kebab-case",
                },
            ],
            'prefer-const': 'error',
            'semi': 'error',
            'padding-line-between-statements': [
                'error',
                {
                    blankLine: 'always',
                    prev: '*',
                    next: 'return'
                }
            ],
            '@typescript-eslint/member-ordering': [
                'error',
                {
                    default: [
                        ["public-static-get", "public-static-set"],
                        ["protected-static-get", "protected-static-set"],
                        ["private-static-get", "private-static-set"],
                        "public-static-field",
                        "protected-static-field",
                        "private-static-field",
                        "public-static-method",
                        "protected-static-method",
                        "private-static-method",
                        ["public-abstract-get", "public-abstract-set"],
                        ["protected-abstract-get", "protected-abstract-set"],
                        "public-abstract-field",
                        "protected-abstract-field",
                        ["public-decorated-get", "public-decorated-set"],
                        "public-decorated-field",
                        ["protected-decorated-get", "protected-decorated-set"],
                        "protected-decorated-field",
                        ["private-decorated-get", "private-decorated-set"],
                        "private-decorated-field",
                        ["public-get", "public-set"],
                        ["protected-get", "protected-set"],
                        ["private-get", "private-set"],
                        "public-field",
                        "protected-field",
                        "private-field",
                        "constructor",
                        "public-abstract-method",
                        "protected-abstract-method",
                        "public-decorated-method",
                        "protected-decorated-method",
                        "private-decorated-method",
                        "public-method",
                        "protected-method",
                        "private-method"
                    ],
                }
            ],
            '@typescript-eslint/naming-convention': [
                'error',
                {
                    selector: 'default',
                    format: ['camelCase'],
                    custom: {
                        regex: '^[^А-ЯЁа-яё]*$',
                        match: true,
                    },
                },
                {
                    selector: ['classProperty', 'parameterProperty'],
                    format: ['camelCase'],
                    modifiers: ['private'],
                    prefix: ['_'],
                    custom: {
                        regex: '^[^А-ЯЁа-яё]*$',
                        match: true,
                    },
                },
                {
                    selector: ['classProperty'],
                    modifiers: ['public', 'static', 'readonly'],
                    format: ['camelCase', 'UPPER_CASE'],
                    custom: {
                        regex: '^[^А-ЯЁа-яё]*$',
                        match: true,
                    },
                },
                {
                    selector: 'interface',
                    format: ['PascalCase'],
                    custom: {
                        regex: '^I[A-Z][^А-Яа-я]*$',
                        match: true,
                    },
                },
                {
                    selector: 'objectLiteralProperty',
                    format: null,
                    custom: {
                        regex: '^[^А-ЯЁа-яё]*$',
                        match: true,
                    },
                },
                {
                    selector: 'typeLike',
                    format: ['PascalCase'],
                    custom: {
                        regex: '^[^А-ЯЁа-яё]*$',
                        match: true,
                    },
                },
                {
                    selector: ['variable'],
                    modifiers: ['const', 'exported'],
                    format: ['camelCase', 'UPPER_CASE'],
                    custom: {
                        regex: '^[^А-ЯЁа-яё]*$',
                        match: true,
                    },
                }
            ],
            '@typescript-eslint/no-shadow': 'error',
            '@typescript-eslint/explicit-member-accessibility': [
                'error',
                {
                    accessibility: 'explicit',
                    overrides: {
                        constructors: 'no-public',
                    },
                }
            ],
            '@typescript-eslint/array-type': [
                'error',
                {
                    default: 'array-simple'
                }
            ],
            '@typescript-eslint/typedef': [
                'error',
                {
                    variableDeclaration: true,
                    arrayDestructuring: true,
                    parameter: true,
                    propertyDeclaration: true,
                    memberVariableDeclaration: true,
                    objectDestructuring: true,
                    variableDeclarationIgnoreFunction: true,
                }
            ],
            '@typescript-eslint/explicit-function-return-type': 'error',
            '@angular-eslint/no-conflicting-lifecycle': 'error',
            '@angular-eslint/no-input-rename': 'error',
            '@angular-eslint/no-inputs-metadata-property': 'error',
            '@angular-eslint/no-output-native': 'error',
            '@angular-eslint/no-output-on-prefix': 'error',
            '@angular-eslint/no-output-rename': 'error',
            '@angular-eslint/no-outputs-metadata-property': 'error',
            '@angular-eslint/use-lifecycle-interface': 'error',
            '@angular-eslint/use-pipe-transform-interface': 'error',
            '@angular-eslint/prefer-on-push-component-change-detection': 'error',
            'jsdoc/require-jsdoc': [
                'error',
                {
                    contexts: [
                        'MethodDefinition:not([key.name="constructor"]):not([key.name="ngOnInit"]):not([key.name="ngOnDestroy"]):not([key.name="ngOnChanges"]):not([key.name="ngDoCheck"]):not([key.name="ngAfterContentInit"]):not([key.name="ngAfterContentChecked"]):not([key.name="ngAfterViewInit"]):not([key.name="ngAfterViewChecked"])'
                    ],
                    require: {
                        ArrowFunctionExpression: false,
                        ClassDeclaration: false,
                        ClassExpression: false,
                        FunctionDeclaration: true,
                        FunctionExpression: true,
                    },
                }
            ],
            "jsdoc/require-description": "error",
            "jsdoc/require-returns": "off",
            'max-classes-per-file': ['error', 1],
            'curly': 'error',
            'eqeqeq': ['error', 'always'],
            'indent': ['error', 4, { SwitchCase: 1 }],
            'quotes': ['error', 'single', { allowTemplateLiterals: true }],
            'object-curly-spacing': ['error', 'always'],
            'unused-imports/no-unused-imports': 'warn',
        },
    },
    {
        files: ["**/*.html"],
        extends: [
            ...angular.configs.templateRecommended,
            ...angular.configs.templateAccessibility,
        ],
        rules: {},
    }
);

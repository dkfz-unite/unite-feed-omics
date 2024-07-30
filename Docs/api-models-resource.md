# Resource Data Model
Includes the information about available analysis resource (file).

**`type`*** - Resource data type.
- Type: _String_
- Available values: `"dna"`, `"dna-ssm"`, `"dna-cnv"`, `"dna-sv"`, `"rna"`, `"rna-exp"`, `"rnasc"`, `"rnasc-exp"`
- Example: `"dna"`

**`format`*** - Resource format.
- Type: _String_
- Available values: `"tsv"`, `"csv"`, `"vcf"`, `"bam"`, `"mex"`
- Example: `"bam"`

**`url`*** - Resource URL on remote a server.
- NOTE: Resource can be gzipped
- Type: _String_
- Example: `"https://example.com/data/d01/scrna/results"`

#### Resource Type
Resource data can be of the following types:
- `"dna"` - DNA alignment data
- `"dna-ssm"` - DNA simple somatic mutations
- `"dna-cnv"` - DNA copy number variants
- `"dna-sv"` - DNA structural variants
- `"rna"` - Bulk RNA alignment data
- `"rna-exp"` - Bulk RNA gene expressions data
- `"rnasc"` - Single cell RNA alignment data
- `"rnasc-exp"` - Single cell RNA gene expressions data

#### Resource Format
Resource format can be of the following types:
- `"tsv"` - Tab-separated values
- `"csv"` - Comma-separated values
- `"vcf"` - Variant Calling Format
- `"BAM"` - Binary Alignment Map
- `"MEX"` - [10xGenomics](https://www.10xgenomics.com/) single cell [dense gene expressions matrix](https://www.10xgenomics.com/support/software/cell-ranger/latest/analysis/outputs/cr-outputs-mex-matrices) (folder)


##
**`*`** - Required fields

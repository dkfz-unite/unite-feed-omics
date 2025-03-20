# Resource Data Model
Includes the information about available analysis resource (file).

**`name`*** - Resource name.
- NOTE: For multi file resources, such as `mtx` or `idat` it's very important to name the files [correctly](#naming). 
- Type: _String_
- Limitations: Maximum length 255
- Example: `"barcodes"`

**`type`*** - Resource data type.
- Type: _String_
- Available values: [Resource Type](#resource-type)
- Example: `"rnasc-exp"`

**`format`*** - Resource format.
- Type: _String_
- Available values: [Resource Format](#resource-format)
- Example: `"tsv"`

**`archive`** - Resource archive type, if the file is archived.
- Type: _String_
- Available values: `"zip"`, `"gz"`
- Example: `"gz"`

**`url`*** - Resource URL on remote a server.
- Type: _String_
- Example: `"https://example.com/file/abcd1234"`

#### Resource Type
Resource data can be of the following types:
- `"dna"` - DNA sample data (fasta, fastq, bam)
- `"dna-ssm"` - DNA simple somatic mutations
- `"dna-cnv"` - DNA copy number variants
- `"dna-sv"` - DNA structural variants
- `"meth"` - DNA methylation sequencing data (fasta, fastq, bam, idat)
- `"meth-lvl"` - DNA methylation levels (beta- and/or M-values)
- `"rna"` - Bulk RNA alignment data (fasta, fastq, bam)
- `"rna-exp"` - Bulk RNA gene expressions data
- `"rnasc"` - Single cell RNA alignment data (fasta, fastq, bam)
- `"rnasc-exp"` - Single cell RNA gene expressions data (mtx)

#### Resource Format
Resource format can be of the following types:
- `"txt"` - Plain text
- `"tsv"` - Tab-separated values
- `"csv"` - Comma-separated values
- `"vcf"` - Variant Calling Format
- `"fasta"` - DNA or RNA sequence data
- `"fastq"` - DNA or RNA sequence data
- `"bam"` - Binary Alignment Map
- `"bam.bai"` - Binary Alignment Map index
- `"bam.bai.md5"` - Binary Alignment Map index MD5 checksum
- `"mtx"` - [10xGenomics](https://www.10xgenomics.com/) single cell [gene expressions matrix](https://www.10xgenomics.com/support/software/cell-ranger/latest/analysis/outputs/cr-outputs-mex-matrices)
- `"idat"` - [Illumina](https://emea.illumina.com) Infinium Methylation array data

#### Naming
- For `mtx` resource type
    - `mtx` file can have any name.
    - `barcodes` file should be named as `barcodes`.
    - `features` file should be named as `features`.
- For `idat` resource type
    - Red channel file should be named as `red`.
    - Green channel file should be named as `green`.
- For other resource types
    - Name the files as you like.

#### Recommendations
- Do not compress alignment files (`bam`, `bam.bai`, `bam.bai.md5`) as they can be used for partial data retrieval.
- Do not decompress `mtx` file and it's `barcodes` and `features` files as they are used in compressed form by the analysis tools.

**Ignoring naming convention and recommendations will make resources unusable**

##
**`*`** - Required fields

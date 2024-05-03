# Resource Data Model
Includes the information about available analysis resource (file).

**`type`*** - Resource type.
- Type: _String_
- Available values: `"MEX"`
- Example: `"MEX"`

**`path`** - Resource path (file or folder) in a hosting environment.
- Notes: Can be compressed using gzip.
- Type: _String_
- Limitations: Should not be empty if `url` is empty.
- Example: `"/data/d01/scrna/results"`

**`url`** - Resource URL on remote a server.
- Type: _String_
- Limitations: Should not be empty if `path` is empty. Only `http` and `https` protocols are allowed for now.
- Example: `"https://example.com/data/d01/scrna/results"`

#### Resource Type
Resource can be of the following types:
- `"MEX"` - folder of the 10xGenomics single cell dense matrix files (features.tsv, barcodes.tsv, matrix.mtx)

##
**`*`** - Required fields

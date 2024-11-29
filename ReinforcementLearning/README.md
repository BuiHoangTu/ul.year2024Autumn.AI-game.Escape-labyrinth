# Reinforcement Learning
This folder is used for implementing AI including but not limited to setting up dev environment.

Check [this video](https://www.youtube.com/watch?v=zPFU30tbyKs) for more information.

## Environment
Using [Micromamba](https://mamba.readthedocs.io/en/latest/installation/micromamba-installation.html) for vscode integration.

### Export 
```bash
micromamba env export -n unity2022-ai > env.yml && echo -e "- pip:\n$(micromamba run -n unity2022-ai pip freeze | sed 's/^/    - /')" >> env.yml
```

### Import 
1. Create environment
    ```bash
    micromamba env create -f env.yml
    ```

    **Note**: For training on AMD gpu, you will need to change `pytorch` version to `rocm` version. For more info, check [pytorch version 1.8.1](https://pytorch.org/get-started/previous-versions/#v181). Version 1.8.2 is not available.

1. Activate
    ```bash
    micromamba activate unity2022-ai
    ```
    
    **Linux**: install `libtinfo5`

## Execute

1. Run `mlagents-learn`
1. Run the scene to train

**Note**: If any error to gpu. Restart system.
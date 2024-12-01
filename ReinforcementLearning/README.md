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

**Note**: 

1. If any error to gpu. Restart system.
1. This is a false alarm. Ignore:

    ```
    UserWarning: Exporting a model to ONNX with a batch_size other than 1, with a variable length with LSTM can cause an error when running the ONNX model with a different batch size. Make sure to save the model with a batch size of 1, or define the initial states (h0/c0) as inputs of the model. 
    warnings.warn("Exporting a model to ONNX with a batch_size other than 1, " +
    ```
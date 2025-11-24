# AI美术生成环境搭建

## 🤖 安装Stable Diffusion WebUI

### 方法1：自动安装脚本 (推荐)

```bash
# 1. 克隆项目
git clone https://github.com/AUTOMATIC1111/stable-diffusion-webui.git
cd stable-diffusion-webui

# 2. 运行安装脚本 (macOS)
./webui.sh

# 首次运行会自动下载模型，需要等待较长时间
```

### 方法2：手动安装

```bash
# 1. 安装Python依赖
pip install torch torchvision torchaudio
pip install diffusers transformers accelerate

# 2. 下载基础模型
# 会自动下载到 ~/.cache/huggingface/
```

## 🚀 启动AI服务

```bash
# 启动WebUI (带API支持)
./webui.sh --api --listen

# 服务地址: http://localhost:7860
# API地址: http://localhost:7860/docs
```

## ✅ 测试连接

打开浏览器访问: http://localhost:7860

看到WebUI界面说明安装成功！

## 📝 注意事项

- 首次下载模型需要4-8GB空间
- 生成图片需要较好的显卡 (推荐8GB+ VRAM)
- 如果没有独立显卡，可以使用CPU模式 (较慢)

## 🎨 测试提示词

在WebUI中测试以下提示词：

```
pixel art character, small person, 16x16 pixels, game sprite, red shirt, simple colors
```

生成成功说明AI环境就绪！

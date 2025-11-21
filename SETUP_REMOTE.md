# 🔗 Demo2 远端仓库设置指南

## 📋 创建GitHub远端仓库步骤

### 方法1：通过GitHub网站创建

1. **访问GitHub**
   - 打开 https://github.com
   - 登录您的GitHub账户

2. **创建新仓库**
   - 点击右上角的 "+" 按钮
   - 选择 "New repository"

3. **配置仓库信息**
   - **Repository name**: `DEMO2`
   - **Description**: `MYH项目集合 - Demo2实验项目`
   - **Visibility**: Public (推荐) 或 Private
   - **不要**勾选 "Add a README file" (我们已经有了)
   - **不要**勾选 "Add .gitignore" (我们已经有了)
   - **不要**勾选 "Choose a license" (我们已经有了)

4. **创建仓库**
   - 点击 "Create repository" 按钮

### 方法2：通过GitHub CLI创建 (如果已安装)

```bash
# 安装GitHub CLI (如果未安装)
# macOS: brew install gh
# Windows: winget install GitHub.cli

# 登录GitHub
gh auth login

# 创建远端仓库
gh repo create DEMO2 --public --description "MYH项目集合 - Demo2实验项目"
```

## 🚀 连接本地仓库到远端

创建远端仓库后，在demo2目录中执行以下命令：

```bash
# 添加远端仓库
git remote add origin https://github.com/MMMMYH/DEMO2.git

# 推送到远端
git branch -M main
git push -u origin main
```

## ✅ 验证设置

推送成功后，您可以：

1. **访问仓库页面**
   ```
   https://github.com/MMMMYH/DEMO2
   ```

2. **验证文件**
   - README.md 应该正确显示
   - .gitignore 文件存在
   - LICENSE 文件存在

3. **克隆测试**
   ```bash
   git clone https://github.com/MMMMYH/DEMO2.git test-clone
   cd test-clone
   ls -la
   ```

## 🔧 后续配置 (可选)

### 设置分支保护规则
1. 进入仓库 Settings > Branches
2. 添加规则保护 main 分支
3. 启用 "Require pull request reviews"

### 配置Issues和Projects
1. 启用 Issues 功能
2. 创建项目看板
3. 设置标签和里程碑

### 添加协作者
1. 进入 Settings > Manage access
2. 点击 "Invite a collaborator"
3. 输入用户名或邮箱

---

**完成这些步骤后，Demo2项目就有了完整的远端仓库！** 🎉

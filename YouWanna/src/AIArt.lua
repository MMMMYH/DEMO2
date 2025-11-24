-- 静态资源加载模块
local AIArt = {}

-- 静态资源配置
AIArt.config = {
    resourceDir = "src/",
    
    -- 资源文件映射
    resources = {
        -- 玩家资源
        player_classic = "player_classic.png",
        
        -- 尖刺资源
        spike_up = "spike_up.png",
        spike_down = "spike_down.png",
        spike_left = "spike_left.png",
        spike_right = "spike_right.png",
        
        -- 背景资源
        sky_background = "sky_background.png",
        cloud_background = "cloud_background.png",
        
        -- 墙面资源
        brick_wall = "brick_wall.png",
        stone_wall = "stone_wall.png",
        
        -- 草皮资源
        grass_top = "grass_top.png",
        grass_side = "grass_side.png",
        
        -- 其他资源
        save_point = "save_point.png"
    }
}

-- 初始化静态资源系统
function AIArt:init()
    print("静态资源系统初始化...")
    
    -- 检查资源文件
    self:checkResources()
    
    print("✅ 静态资源系统初始化完成")
end

-- 检查资源文件
function AIArt:checkResources()
    local missingFiles = {}
    
    for resourceName, fileName in pairs(self.config.resources) do
        local filePath = self.config.resourceDir .. fileName
        if not cc.FileUtils:getInstance():isFileExist(filePath) then
            table.insert(missingFiles, fileName)
        end
    end
    
    if #missingFiles > 0 then
        print("⚠️ 缺少资源文件: " .. table.concat(missingFiles, ", "))
    else
        print("✅ 所有资源文件检查完成")
    end
end

-- 直接加载静态资源
function AIArt:loadStaticResource(resourceName, callback)
    local fileName = self.config.resources[resourceName]
    if not fileName then
        print("❌ 未找到资源: " .. resourceName)
        if callback then
            callback(nil, false)
        end
        return
    end
    
    local filePath = self.config.resourceDir .. fileName
    if cc.FileUtils:getInstance():isFileExist(filePath) then
        print("✅ 加载静态资源: " .. resourceName)
        if callback then
            callback(filePath, true)
        end
    else
        print("❌ 资源文件不存在: " .. filePath)
        if callback then
            callback(nil, false)
        end
    end
end

-- 加载玩家角色
function AIArt:generatePlayer(style, callback)
    style = style or "classic"
    self:loadStaticResource("player_" .. style, callback)
end

-- 加载障碍物
function AIArt:generateObstacle(type, callback)
    self:loadStaticResource("spike_" .. type, callback)
end

-- 加载背景
function AIArt:generateBackground(theme, callback)
    self:loadStaticResource(theme .. "_background", callback)
end

-- 资源加载管理器
AIArt.loadedTextures = {}

-- 加载静态纹理
function AIArt:loadTexture(filename, callback)
    -- 检查是否已经加载
    if self.loadedTextures[filename] then
        print("📦 使用已加载纹理: " .. filename)
        if callback then
            callback(self.loadedTextures[filename])
        end
        return
    end
    
    -- 直接从src文件夹加载
    local resourceName = filename
    local fileName = self.config.resources[resourceName]
    
    if not fileName then
        print("❌ 未找到资源映射: " .. filename)
        if callback then
            callback(nil)
        end
        return
    end
    
    local filePath = self.config.resourceDir .. fileName
    if not cc.FileUtils:getInstance():isFileExist(filePath) then
        print("❌ 纹理文件不存在: " .. filePath)
        if callback then
            callback(nil)
        end
        return
    end
    
    -- 加载纹理
    local texture = cc.Director:getInstance():getTextureCache():addImage(filePath)
    if texture then
        self.loadedTextures[filename] = texture
        print("✅ 静态纹理加载成功: " .. filename)
        if callback then
            callback(texture)
        end
    else
        print("❌ 纹理加载失败: " .. filename)
        if callback then
            callback(nil)
        end
    end
end

-- 创建精灵从AI生成的纹理
function AIArt:createSprite(filename, callback)
    self:loadTexture(filename, function(texture)
        if texture then
            local sprite = cc.Sprite:createWithTexture(texture)
            if callback then
                callback(sprite)
            end
        else
            -- 创建占位精灵
            local sprite = cc.Sprite:create()
            local drawNode = cc.DrawNode:create()
            local size = cc.size(32, 32)
            local color = cc.c4f(0.8, 0.8, 0.8, 1)
            local vertices = {
                cc.p(0, 0), cc.p(size.width, 0),
                cc.p(size.width, size.height), cc.p(0, size.height)
            }
            drawNode:drawPolygon(vertices, color, 1, color)
            sprite:addChild(drawNode)
            
            if callback then
                callback(sprite)
            end
        end
    end)
end

-- 批量预加载静态资源
function AIArt:preGenerateAssets(callback)
    print("📦 开始预加载静态资源...")
    
    local assetsToLoad = {
        "player_classic",
        "spike_up",
        "spike_down", 
        "spike_left",
        "spike_right",
        "sky_background"
    }
    
    local completed = 0
    local total = #assetsToLoad
    
    local function onAssetComplete()
        completed = completed + 1
        print(string.format("📦 资源加载进度: %d/%d", completed, total))
        
        if completed >= total then
            print("✅ 所有静态资源预加载完成!")
            if callback then
                callback()
            end
        end
    end
    
    -- 开始加载
    for _, resourceName in ipairs(assetsToLoad) do
        self:loadTexture(resourceName, function(texture)
            onAssetComplete()
        end)
    end
end

return AIArt

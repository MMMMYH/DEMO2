-- 障碍物系统
local ObstacleSystem = {}

function ObstacleSystem:init(scene, aiArt)
    self.scene = scene
    self.aiArt = aiArt
    self.obstacles = {}
    
    print("障碍物系统初始化完成")
end

-- 创建障碍物
function ObstacleSystem:createObstacle(type, x, y, callback)
    local obstacle = {
        type = type,
        x = x,
        y = y,
        width = 32,
        height = 32,
        sprite = nil,
        deadly = true
    }
    
    -- 根据类型设置属性
    if type == "spike_wall" then
        obstacle.width = 32
        obstacle.height = 64
        obstacle.deadly = true
    elseif type:match("spike_") then
        obstacle.width = 32
        obstacle.height = 32
        obstacle.deadly = true
    end
    
    -- 创建静态精灵资源
    self.aiArt:createSprite("spike_" .. type, function(sprite)
        if sprite then
            obstacle.sprite = sprite
            sprite:setPosition(x, y)
            self.scene:addChild(sprite)
            
            -- 添加到障碍物列表
            table.insert(self.obstacles, obstacle)
            
            print("✅ 静态障碍物创建完成: " .. type)
            
            if callback then
                callback(obstacle)
            end
        else
            -- 创建备用障碍物
            self:createFallbackObstacle(obstacle, callback)
        end
    end)
end

-- 创建备用障碍物
function ObstacleSystem:createFallbackObstacle(obstacle, callback)
    local drawNode = cc.DrawNode:create()
    
    -- 所有尖刺都用橙色
    local color = cc.c4f(1, 0.5, 0, 1) -- 橙色
    
    -- 根据尖刺方向绘制不同形状
    if obstacle.type == "spike_up" then
        -- 向上的三角形
        local vertices = {
            cc.p(16, 32), -- 顶点
            cc.p(0, 0),   -- 左下
            cc.p(32, 0)   -- 右下
        }
        drawNode:drawPolygon(vertices, color, 1, color)
    elseif obstacle.type == "spike_down" then
        -- 向下的三角形
        local vertices = {
            cc.p(16, 0),  -- 底点
            cc.p(0, 32),  -- 左上
            cc.p(32, 32)  -- 右上
        }
        drawNode:drawPolygon(vertices, color, 1, color)
    elseif obstacle.type == "spike_left" then
        -- 向左的三角形
        local vertices = {
            cc.p(0, 16),  -- 左点
            cc.p(32, 0),  -- 右下
            cc.p(32, 32)  -- 右上
        }
        drawNode:drawPolygon(vertices, color, 1, color)
    elseif obstacle.type == "spike_right" then
        -- 向右的三角形
        local vertices = {
            cc.p(32, 16), -- 右点
            cc.p(0, 0),   -- 左下
            cc.p(0, 32)   -- 左上
        }
        drawNode:drawPolygon(vertices, color, 1, color)
    elseif obstacle.type == "spike_cluster" then
        -- 多个小尖刺
        local spikes = {
            {cc.p(8, 16), cc.p(0, 0), cc.p(16, 0)},
            {cc.p(24, 16), cc.p(16, 0), cc.p(32, 0)},
            {cc.p(16, 32), cc.p(8, 16), cc.p(24, 16)}
        }
        for _, spike in ipairs(spikes) do
            drawNode:drawPolygon(spike, color, 1, color)
        end
    else
        -- 默认向上尖刺
        local vertices = {
            cc.p(16, 32),
            cc.p(0, 0),
            cc.p(32, 0)
        }
        drawNode:drawPolygon(vertices, color, 1, color)
    end
    
    obstacle.sprite = drawNode
    drawNode:setPosition(obstacle.x, obstacle.y)
    self.scene:addChild(drawNode)
    
    -- 添加到障碍物列表
    table.insert(self.obstacles, obstacle)
    
    print("🔄 备用障碍物创建完成: " .. obstacle.type)
    
    if callback then
        callback(obstacle)
    end
end

-- 检查玩家与障碍物碰撞
function ObstacleSystem:checkCollision(playerRect)
    for _, obstacle in ipairs(self.obstacles) do
        if obstacle.sprite then
            local obstacleRect = cc.rect(
                obstacle.x - obstacle.width / 2,
                obstacle.y - obstacle.height / 2,
                obstacle.width,
                obstacle.height
            )
            
            if cc.rectIntersectsRect(playerRect, obstacleRect) then
                if obstacle.deadly then
                    return "death" -- 致命碰撞
                elseif obstacle.type == "platform" then
                    return "platform" -- 平台碰撞
                end
            end
        end
    end
    
    return nil
end

-- 创建测试关卡 - 基于经典I Wanna截图
function ObstacleSystem:createTestLevel()
    print("🎮 创建I Wanna风格测试关卡...")
    
    -- === 底层区域 (地面层) ===
    -- 底部地面尖刺
    self:createObstacle("spike_up", 160, 550)
    self:createObstacle("spike_up", 320, 550)
    self:createObstacle("spike_up", 480, 550)
    self:createObstacle("spike_up", 640, 550)
    
    -- === 第一层平台 (高度400) ===
    -- 平台上的向上尖刺
    self:createObstacle("spike_up", 200, 400)
    self:createObstacle("spike_up", 264, 400)
    
    -- 平台下方的向下尖刺
    self:createObstacle("spike_down", 150, 350)
    self:createObstacle("spike_down", 300, 350)
    
    -- === 第二层平台 (高度300) ===
    -- 左侧墙壁向右尖刺
    self:createObstacle("spike_right", 32, 300)
    self:createObstacle("spike_right", 32, 250)
    
    -- 平台间隙的尖刺陷阱
    self:createObstacle("spike_up", 400, 300)
    self:createObstacle("spike_down", 450, 250)
    
    -- === 第三层平台 (高度200) ===
    -- 右侧墙壁向左尖刺
    self:createObstacle("spike_left", 768, 200)
    self:createObstacle("spike_left", 768, 150)
    
    -- 小平台上的尖刺组合
    self:createObstacle("spike_up", 350, 200)
    self:createObstacle("spike_up", 382, 200)
    
    -- === 顶层区域 (高度100) ===
    -- 天花板向下尖刺
    self:createObstacle("spike_down", 200, 50)
    self:createObstacle("spike_down", 400, 50)
    self:createObstacle("spike_down", 600, 50)
    
    -- === 中央危险区域 ===
    -- 悬空的尖刺集群
    self:createObstacle("spike_cluster", 500, 180)
    
    -- 交叉尖刺陷阱
    self:createObstacle("spike_up", 550, 350)
    self:createObstacle("spike_down", 580, 280)
    self:createObstacle("spike_left", 650, 320)
    
    -- === 隐蔽陷阱 ===
    -- 看似安全的跳跃点
    self:createObstacle("spike_up", 120, 450) -- 起始区陷阱
    self:createObstacle("spike_right", 100, 380) -- 墙角陷阱
    self:createObstacle("spike_down", 700, 380) -- 高处陷阱
    
    -- === 终点前的最后挑战 ===
    -- 密集尖刺阵列
    self:createObstacle("spike_up", 680, 450)
    self:createObstacle("spike_up", 712, 450)
    self:createObstacle("spike_left", 750, 420)
    self:createObstacle("spike_down", 720, 380)
    
    print("I Wanna风格测试关卡创建完成 - 包含" .. #self.obstacles .. "个尖刺陷阱!")
    print("关卡特色: 多层平台结构 + 各方向尖刺 + 隐蔽陷阱")
end

-- 清除所有障碍物
function ObstacleSystem:clear()
    for _, obstacle in ipairs(self.obstacles) do
        if obstacle.sprite then
            obstacle.sprite:removeFromParent()
        end
    end
    self.obstacles = {}
    
    print("障碍物清除完成")
end

return ObstacleSystem

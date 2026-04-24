-- CREATE LOGS TABLE ON TiDB CLOUD (MySQL Compatible)
CREATE TABLE IF NOT EXISTS cloud_sync_logs (
    id VARCHAR(36) PRIMARY KEY,
    entity_name VARCHAR(100),
    entity_id VARCHAR(100),
    action VARCHAR(50),
    data_json JSON,
    created_at DATETIME,
    INDEX idx_entity (entity_name, entity_id),
    INDEX idx_created (created_at)
) ENGINE=InnoDB;

-- Tối ưu hóa cho Write Load cực lớn trên TiDB
-- TiDB tự động xử lý Sharding dựa trên Primary Key (Region).
-- Nếu muốn hiệu năng ghi cao hơn nữa, có thể sử dụng AUTO_RANDOM cho PK.

import React from "react";
import { Link } from "react-router-dom";
import SearchBar from "./SearchBar";
import { AppBar, Toolbar, Typography, Box, TextField, Button } from "@mui/material";

const Navbar = ({ searchQuery, setSearchQuery, handleSearch }) => {
    return (
      <AppBar position="static" sx={{ backgroundColor: "#0b0f19", padding: 1 }}>
        <Toolbar sx={{ display: "flex", justifyContent: "space-between", alignItems: "center" }}>
          <Typography variant="h6" sx={{ fontFamily: "'Cinzel Decorative', serif" }}>
            MYTHICHUB
          </Typography>
          {/* Search Bar */}
          <Box sx={{ display: "flex", alignItems: "center", gap: 1 }}>
            <TextField
              variant="outlined"
              size="small"
              placeholder="Search character..."
              value={searchQuery}
              onChange={(e) => setSearchQuery(e.target.value)}
              sx={{ backgroundColor: "white", borderRadius: 1 }}
            />
            <Button variant="contained" color="primary" onClick={handleSearch}>
              SEARCH
            </Button>
          </Box>
          {/* Navigation Links */}
          <Box sx={{ display: "flex", gap: 2 }}>
            <Typography variant="body1" sx={{ cursor: "pointer" }}>
              Guides
            </Typography>
            <Typography variant="body1" sx={{ cursor: "pointer" }}>
              Compare
            </Typography>
          </Box>
        </Toolbar>
      </AppBar>
    );
  };
  
  export default Navbar;

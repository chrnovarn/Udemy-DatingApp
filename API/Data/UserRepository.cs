using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public UserRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await _context.Users
            .Include(p => p.Photos)
            .ToListAsync();
        }

        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            return await _context.Users
            .FindAsync(id);
        }

        public async Task<AppUser> GetUserByUsernameAsync(string username)
        {
            return await _context.Users
            .Include(p => p.Photos)
            .SingleOrDefaultAsync(x => x.UserName == username);
        }

        public void Update(AppUser user)
        {
            _context.Entry(user).State = EntityState.Modified;
        }

        public async Task<PagedList<memberDTO>> GetMembersAsync(UserParams userParams)
        {
            var query =  _context.Users.AsQueryable();
            
            query = query.Where(u => u.UserName != userParams.CurrentUserName);
            query = query.Where(u => u.Gender == userParams.Gender);
            
            var minDob = DateTime.Today.AddYears(-userParams.MaxAge - 1);
            var maxDob = DateTime.Today.AddYears(-userParams.MinAge);

            query = query.Where(u => u.DateOfBirth >= minDob & u.DateOfBirth <= maxDob);
            query = userParams.OrderBy switch
            {
                "created" => query.OrderByDescending(u => u.Created),
                _ => query.OrderByDescending(u =>u.LastActive)
            };

            return await PagedList<memberDTO>.CreateAsync(
                query.ProjectTo<memberDTO>(_mapper.ConfigurationProvider).AsNoTracking(),
                userParams.PageNumber,
                userParams.PageSize
                );
            
        }

        public async Task<memberDTO> GetMemberAsync(string username)
        {
            return await _context.Users
                .Where(x => x.UserName == username)
                .ProjectTo<memberDTO>(_mapper.ConfigurationProvider).SingleOrDefaultAsync();
        }

        public async Task<string> GetUserGender(string username)
        {
            return await _context.Users
            .Where(u => u.UserName == username)
            .Select(s => s.Gender).FirstOrDefaultAsync();
        }
    }
}